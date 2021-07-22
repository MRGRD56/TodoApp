import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import TodoItem from "../../models/TodoItem";
import { fromEvent, Observable } from "rxjs";

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
})
export class HomeComponent {
    public isTodosLoading = false;
    public isNewTodoSending = false;
    public isAllTodosLoaded = false;

    public newTodoText = "";

    public todoItems: TodoItem[] = [];

    private lastLoadedPage: number = -1;

    constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) {
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
                todoItems.forEach(todoItem => this.todoItems.push(todoItem));
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
        this.http.post<TodoItem>(this.baseUrl + "api/todo", postBody).subscribe(addedTodo => {
            this.newTodoText = "";
            this.todoItems.unshift(addedTodo);
            this.isNewTodoSending = false;
        });
    }
}
