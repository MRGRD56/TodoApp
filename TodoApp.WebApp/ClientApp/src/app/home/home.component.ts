import { Component, Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import TodoItem from "../../models/TodoItem";
import { fromEvent, Observable } from "rxjs";
import { TodoHubService } from "../todo-hub.service";
import Checkable from "../../models/Checkable";
import { HubConnectionState } from "@microsoft/signalr";
import ResizeObserver from 'resize-observer-polyfill';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['home.component.scss']
})
export class HomeComponent {
    public isTodosLoading = false;
    public isNewTodoSending = false;
    public isAllTodosLoaded = false;
    public isDoneToggling = false;
    public isTodosDeleting = false;

    public newTodoText = "";

    public todoItems: Checkable<TodoItem>[] = [];

    public get selectedTodoItems(): TodoItem[] {
        return this.todoItems
            .filter(item => item.isChecked)
            .map(item => item.item);
    }

    private lastLoadedPage: number = -1;

    private bodyResizeObserver: ResizeObserver = new ResizeObserver(entries => {
        const isScrolledToBottom = HomeComponent.isScrolledToBottom();
        console.log("Body resized, isScrolledToBottom:", isScrolledToBottom);
        if (isScrolledToBottom && !this.isTodosLoading) {
            console.log("bodyResizeObserver: fetching");
            this.fetchTodoItems().then(r => {
                console.log("bodyResizeObserver: fetching completed");
            });
        }
    });

    constructor(private readonly http: HttpClient,
                private readonly todoHubService: TodoHubService,
                @Inject("BASE_URL") private readonly baseUrl: string) {
        this.initialize();
    }

    private async initialize() {
        if (this.todoHubService.hubConnection.state === HubConnectionState.Disconnected) {
            this.isTodosLoading = true;
            await this.todoHubService.hubConnection.start();
        }

        this.todoHubService.hubConnection.on("Add", (newTodoItem: TodoItem) => {
            this.todoItems.unshift(new Checkable<TodoItem>(newTodoItem));
        });

        this.todoHubService.hubConnection.on("Delete", (deletedTodoItemsId: number[]) => {
            deletedTodoItemsId.forEach(id => {
                const itemToDelete = this.todoItems.find(x => x.item.id === id);
                if (itemToDelete) {
                    const index = this.todoItems.indexOf(itemToDelete);
                    this.todoItems.splice(index, 1);
                }
            });
        });

        this.todoHubService.hubConnection.on("ToggleDone", (editedTodoItems: TodoItem[]) => {
            editedTodoItems.forEach(editedItem => {
                const itemToEdit = this.todoItems.find(x => x.item.id == editedItem.id);
                if (itemToEdit) {
                    itemToEdit.item.isDone = editedItem.isDone;
                }
            })
        });

        const scroll$ = fromEvent(window, "scroll");
        scroll$.subscribe(async _ => {
            const isScrolledToBottom = HomeComponent.isScrolledToBottom();
            console.log("scroll$: next", isScrolledToBottom, this.isAllTodosLoaded, this.isTodosLoading);
            if (isScrolledToBottom && !this.isAllTodosLoaded && !this.isTodosLoading) {
                console.log("scroll$: fetching");
                await this.fetchTodoItems();
            }
        });
        await this.fetchTodoItems();

        //this.bodyResizeObserver.observe(document.body);
    }

    public fetchTodoItems(): Promise<void> {
        return new Promise<void>(resolve => {
            if (this.isAllTodosLoaded) {
                resolve();
                return;
            }

            this.isTodosLoading = true;

            this.getTodoItems$(this.lastLoadedPage + 1).subscribe(async todoItems => {
                if (todoItems.length > 0) {
                    todoItems.forEach(todoItem => this.todoItems.push(new Checkable<TodoItem>(todoItem)));
                    this.lastLoadedPage++;
                } else {
                    this.isAllTodosLoaded = true;
                }
                console.log(`FETCHED PAGE ${this.lastLoadedPage}, ${todoItems.length} ITEMS: ${JSON.stringify(todoItems.map(i => i.id))}`);
                this.isTodosLoading = false;
                setTimeout(async () => {
                    if (HomeComponent.isScrolledToBottom()) {
                        console.log("scrolled to bottom, fetching more items");
                        await this.fetchTodoItems();
                    } else {
                        console.log("FETCHING COMPLETED");
                        resolve();
                    }
                }, 200);
            });
        });
    }

    private getTodoItems$(page: number): Observable<TodoItem[]> {
        return this.http.get<TodoItem[]>(this.baseUrl + `api/todo?page=${page}`);
    }

    private static isScrolledToBottom() {
        return (window.innerHeight + window.scrollY + 15) >= document.body.offsetHeight;
    }

    public addTodoItem() {
        const postBody = {
            text: this.newTodoText
        };

        if (!postBody.text || !postBody.text.trim()) {
            return;
        }

        this.isNewTodoSending = true;
        this.http.post<TodoItem>(this.baseUrl + "api/todo", postBody).subscribe(async addedTodo => {
            this.newTodoText = "";
            await this.todoHubService.hubConnection.invoke("Add", addedTodo);
            this.isNewTodoSending = false;
        });
    }

    public onTodoItemClick(todoItem: Checkable<TodoItem>, e: MouseEvent) {
        if (window.getSelection().toString()
            && window.getSelection().anchorNode.parentNode.parentNode.parentNode === e.currentTarget) {
            return;
        }

        todoItem.isChecked = !todoItem.isChecked;
    }

    public unselectAll() {
        this.todoItems
            .filter(item => item.isChecked)
            .forEach(item => item.isChecked = false);
    }

    public deleteSelectedItems() {
        this.isTodosDeleting = true;

        const requestBody = {
            id: this.selectedTodoItems.map(item => item.id)
        };
        this.http.request<TodoItem[]>("delete", this.baseUrl + "api/todo", {
            body: requestBody
        }).subscribe(async deletedTodos => {
            await this.todoHubService.hubConnection.invoke("Delete", requestBody.id);
            this.isTodosDeleting = false;

            // if (HomeComponent.isScrolledToBottom()) {
            //     await this.fetchTodoItems();
            // }
        });
    }

    public toggleSelectedItemsIsDone() {
        this.isDoneToggling = true;
        this.http.put<TodoItem[]>(this.baseUrl + "api/todo/toggle_done", {
            id: this.selectedTodoItems.map(item => item.id)
        }).subscribe(async editedTodos => {
            await this.todoHubService.hubConnection.invoke("ToggleDone", editedTodos);
            this.isDoneToggling = false;
        });
    }
}
