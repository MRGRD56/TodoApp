import { Component, Inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import TodoItem from "../../models/TodoItem";
import { fromEvent, Observable } from "rxjs";
import { TodoHubService } from "../todo-hub.service";
import Checkable from "../../models/Checkable";
import { HubConnectionState } from "@microsoft/signalr";

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: [ 'home.component.scss' ]
})
export class HomeComponent {
    public isTodosLoading = false;
    public isNewTodoSending = false;
    public isAllTodosLoaded = false;

    public newTodoText = "";

    public todoItems: Checkable<TodoItem>[] = [];
    public get checkedTodoItems(): TodoItem[] {
        return this.todoItems
            .filter(item => item.isChecked)
            .map(item => item.item);
    }

    private lastLoadedPage: number = -1;

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

        this.todoHubService.hubConnection.on("Add", newTodoItem => {
            this.todoItems.unshift(new Checkable<TodoItem>(newTodoItem));
        });

        this.todoHubService.hubConnection.on("Delete", deletedTodoItemId => {

        });

        this.fetchTodoItems();
        const scroll$ = fromEvent(window, "scroll")
        scroll$.subscribe(_ => {
            if (HomeComponent.isScrolledToBottom() && !this.isAllTodosLoaded && !this.isTodosLoading) {
                this.fetchTodoItems();
            }
        });
    }

    private fetchTodoItems(): void {
        this.isTodosLoading = true;
        this.getTodoItems$(this.lastLoadedPage + 1).subscribe(todoItems => {
            if (todoItems.length > 0) {
                todoItems.forEach(todoItem => this.todoItems.push(new Checkable<TodoItem>(todoItem)));
                this.lastLoadedPage++;
            } else {
                this.isAllTodosLoaded = true;
            }
            this.isTodosLoading = false;
        });
    }

    private getTodoItems$(page: number): Observable<TodoItem[]> {
        return this.http.get<TodoItem[]>(this.baseUrl + `api/todo?page=${page}`);
    }

    private static isScrolledToBottom() {
        return (window.innerHeight + window.scrollY) >= document.body.offsetHeight;
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
        if (window.getSelection().toString() && window.getSelection().anchorNode.parentNode.parentNode.parentNode === e.currentTarget) {
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
        this.http.delete<TodoItem>(this.baseUrl + "api/todo").subscribe(async deletedTodo => {

        })
    }
}
