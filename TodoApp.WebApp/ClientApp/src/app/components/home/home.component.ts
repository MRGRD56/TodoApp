import {Component, Inject} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import TodoItem from "../../models/TodoItem";
import {fromEvent, Observable} from "rxjs";
import Checkable from "../../models/Checkable";
import {HubConnectionState} from "@microsoft/signalr";
import {delay} from "../../extensions/AsyncExtensions";
import {TodoHubService} from "../../services/todo-hub/todo-hub.service";
import {TodoService} from "../../services/todo/todo.service";
import { AuthService } from "../../services/auth/auth.service";

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
                @Inject("BASE_URL") private readonly baseUrl: string,
                private readonly todoService: TodoService,
                private readonly auth: AuthService) {
        this.initialize();
    }

    private get currentUserId() {
        return this.auth.currentUser.id.toString();
    }

    private async initialize() {
        TodoHubService.newConnectionStarted.subscribe(async () => {
            await this.initializeTodoHub();
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

        const todoItems = await this.getTodoItems(this.lastLoadedItemId);
        if (todoItems.length > 0) {
            todoItems.forEach(todoItem => this.todoItems.push(new Checkable<TodoItem>(todoItem)));
            this.lastLoadedItemId = todoItems[todoItems.length - 1].id;
        } else {
            this.isAllTodosLoaded = true;
        }
        this.isTodosLoading = false;

        await delay(200);
        if (HomeComponent.isScrolledToBottom()) {
            await this.fetchTodoItems();
        }
    }

    private async getTodoItems(afterId: number | null) {
        return await this.todoService.get(afterId);
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
        const addedTodo = await this.todoService.add(postBody);
        await this.todoHubService.hubConnection.invoke("Add", addedTodo, this.currentUserId);
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
            const editedTodoItem = await this.todoService.edit(this.editingTodoItem.id, putBody);

            await this.todoHubService.hubConnection.invoke("Edit", editedTodoItem, this.currentUserId);
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

    public async deleteSelectedItems() {
        this.isTodosDeleting = true;

        const idsToDelete = this.selectedTodoItems.map(item => item.id);
        const deletedTodos = await this.todoService.deleteItems(idsToDelete);
        await this.todoHubService.hubConnection.invoke("Delete", idsToDelete, this.currentUserId);
        this.isTodosDeleting = false;
    }

    public async toggleSelectedItemsIsDone() {
        this.isDoneToggling = true;
        const editedTodos = await this.todoService.toggleDone(this.selectedTodoItems.map(item => item.id));
        await this.todoHubService.hubConnection.invoke("ToggleDone", editedTodos, this.currentUserId);
        this.isDoneToggling = false;
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
