import { Component, Inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import TodoItem from "../../models/TodoItem";
import { fromEvent, Observable } from "rxjs";
import { TodoHubService } from "../todo-hub.service";
import Checkable from "../../models/Checkable";
import { HubConnectionState } from "@microsoft/signalr";
import ResizeObserver from 'resize-observer-polyfill';
import { delay } from "../../extensions/AsyncExtensions";

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['home.component.scss']
})
export class HomeComponent {
    public isTodosLoading = false;
    public isTodoItemSubmitting = false;
    public isAllTodosLoaded = false;
    public isDoneToggling = false;
    public isTodosDeleting = false;

    private reservedNewTodoText = "";
    public newTodoText = "";

    public todoItems: Checkable<TodoItem>[] = [];


    private _editingTodoItem: TodoItem = null;
    public get editingTodoItem(): TodoItem {
        return this._editingTodoItem;
    }

    public set editingTodoItem(value) {
        this._editingTodoItem = value;

        if (value) {
            this.reservedNewTodoText = this.newTodoText;
            this.newTodoText = this._editingTodoItem.text;
        } else {
            this.newTodoText = this.reservedNewTodoText;
            this.reservedNewTodoText = "";
        }
    }

    public isEditMode(): boolean {
        return this.editingTodoItem != null;
    }

    public get selectedTodoItems(): TodoItem[] {
        return this.todoItems
            .filter(item => item.isChecked)
            .map(item => item.item);
    }

    private lastLoadedItemId: number | null = null;

    constructor(private readonly http: HttpClient,
                private readonly todoHubService: TodoHubService,
                @Inject("BASE_URL") private readonly baseUrl: string) {
        this.initialize();
    }

    private async initialize() {
        await this.initializeTodoHub();

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
    }

    private async initializeTodoHub() {
        if (this.todoHubService.hubConnection.state === HubConnectionState.Disconnected) {
            this.isTodosLoading = true;
            await this.todoHubService.hubConnection.start();
        }

        this.todoHubService.hubConnection.on("Add", (newTodoItem: TodoItem) => {
            this.todoItems.unshift(new Checkable<TodoItem>(newTodoItem));
        });

        this.todoHubService.hubConnection.on("Delete", async (deletedTodoItemsId: number[]) => {
            deletedTodoItemsId.forEach(id => {
                const itemToDelete = this.todoItems.find(x => x.item.id === id);
                if (itemToDelete) {
                    const index = this.todoItems.indexOf(itemToDelete);
                    this.todoItems.splice(index, 1);
                }
            });

            await delay(200);
            if (HomeComponent.isScrolledToBottom()) {
                await this.fetchTodoItems();
            }
        });

        this.todoHubService.hubConnection.on("ToggleDone", (editedTodoItems: TodoItem[]) => {
            editedTodoItems.forEach(editedItem => {
                const itemToEdit = this.todoItems.find(x => x.item.id == editedItem.id);
                if (itemToEdit) {
                    itemToEdit.item.isDone = editedItem.isDone;
                }
            });
        });

        this.todoHubService.hubConnection.on("Edit", (editedTodoItem: TodoItem) => {
            const itemToEdit = this.todoItems.find(x => x.item.id == editedTodoItem.id);
            if (itemToEdit) {
                itemToEdit.item.text = editedTodoItem.text;
            }
        });
    }

    public async fetchTodoItems(): Promise<void> {
        if (this.isAllTodosLoaded) {
            return;
        }

        this.isTodosLoading = true;

        this.getTodoItems$(this.lastLoadedItemId).subscribe(async todoItems => {
            const previousLastLoadedItemId = this.lastLoadedItemId;
            if (todoItems.length > 0) {
                todoItems.forEach(todoItem => this.todoItems.push(new Checkable<TodoItem>(todoItem)));
                this.lastLoadedItemId = todoItems[todoItems.length - 1].id;
            } else {
                this.isAllTodosLoaded = true;
            }
            console.log("FETCHED AFTER ID", previousLastLoadedItemId, ` ${todoItems.length} ITEMS:`, todoItems.map(i => i.id));
            this.isTodosLoading = false;

            await delay(200);
            if (HomeComponent.isScrolledToBottom()) {
                console.log("scrolled to bottom, fetching more items");
                await this.fetchTodoItems();
            } else {
                console.log("FETCHING COMPLETED");
            }
        });
    }

    private getTodoItems$(afterId: number | null): Observable<TodoItem[]> {
        return this.http.get<TodoItem[]>(this.baseUrl + `api/todo/get_after?afterId=${afterId ?? 0}`);
    }

    private static isScrolledToBottom() {
        return (window.innerHeight + window.scrollY + 15) >= document.body.offsetHeight;
    }

    public async submitTodoItem() {
        if (this.isEditMode()) {
            await this.editTodoItem();
        } else {
            await this.addTodoItem();
        }
    }

    public async addTodoItem(): Promise<void> {
        const postBody = {
            text: this.newTodoText
        };

        if (!postBody.text || !postBody.text.trim()) {
            return;
        }

        this.isTodoItemSubmitting = true;
        const addedTodo = await this.http.post<TodoItem>(this.baseUrl + "api/todo", postBody).toPromise()
        await this.todoHubService.hubConnection.invoke("Add", addedTodo);
        this.newTodoText = "";
        this.isTodoItemSubmitting = false;
    }

    public async editTodoItem(): Promise<void> {
        const putBody = {
            text: this.newTodoText
        };

        if (!putBody.text || !putBody.text.trim()) {
            return;
        }

        this.isTodoItemSubmitting = true;

        try {
            const editedTodoItem = await this.http
                .put<TodoItem>(this.baseUrl + `api/todo/${this.editingTodoItem.id}`, putBody)
                .toPromise();

            await this.todoHubService.hubConnection.invoke("Edit", editedTodoItem);
            this.editingTodoItem = null;
            this.isTodoItemSubmitting = false;
        } catch (error) {
            if (error instanceof HttpErrorResponse) {
                const httpErrorResponse = <HttpErrorResponse>error;
                if (httpErrorResponse.status === 400 &&
                    httpErrorResponse.error === "Cannot edit deleted todo") {
                    await this.addTodoItem();
                    this.editingTodoItem = null;
                }
            }
        }
    }

    public onTodoItemClick(todoItem: Checkable<TodoItem>, e: MouseEvent) {
        if (this.isEditMode()
            || window.getSelection().toString()
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

    public startEditMode() {
        if (this.selectedTodoItems.length !== 1) {
            return;
        }

        this.editingTodoItem = this.selectedTodoItems[0];
        document.getElementById("new-todo-textarea").focus();
    }

    public stopEditMode() {
        if (!this.isEditMode()) return;

        this.editingTodoItem = null;
    }
}
