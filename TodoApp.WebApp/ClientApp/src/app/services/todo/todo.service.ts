import {Inject, Injectable} from '@angular/core';
import TodoItem from "../../models/TodoItem";
import {HttpClient} from "@angular/common/http";

@Injectable({
    providedIn: 'root'
})
export class TodoService {

    constructor(private readonly http: HttpClient,
                @Inject("BASE_URL") private readonly baseUrl: string) {
    }

    public async get(afterId: number) {
        return await this.http.get<TodoItem[]>(this.baseUrl + `api/todo/get_after?afterId=${afterId ?? 0}`)
            .toPromise();
    }

    public async add(postBody: {text: string}) {
        return await this.http.post<TodoItem>(`${this.baseUrl}api/todo`, postBody).toPromise()
    }

    public async edit(id: number, putBody: {text: string | undefined}) {
        return await this.http.put<TodoItem>(`${this.baseUrl}api/todo/${id}`, putBody).toPromise();
    }

    public async deleteItems(ids: number[]) {
        return await this.http.request<TodoItem[]>("delete", `${this.baseUrl}api/todo`, {
            body: {
                id: ids
            }
        }).toPromise();
    }

    public async toggleDone(ids: number[]) {
        return await this.http.put<TodoItem[]>(`${this.baseUrl}api/todo/toggle_done`, {
            id: ids
        }).toPromise();
    }
}
