import {Injectable} from '@angular/core';
import {HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState} from "@microsoft/signalr";
import { AuthService } from "../auth/auth.service";
import { Observable, Subscriber } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class TodoHubService {
    public hubConnection: HubConnection;

    public async ensureConnected() {
        if (this.hubConnection == null) return;
        if (this.hubConnection.state == HubConnectionState.Disconnected) {
            await this.hubConnection.start();
        }
    }

    public async startNewConnection() {
        if (!this.auth.isAuthenticated()) {
            throw new Error("Failed to create a hub connection because the user is unauthorized");
        }

        this.hubConnection = new HubConnectionBuilder()
            .withUrl("/hubs/todo", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets,
                withCredentials: true,
                accessTokenFactory(): string | Promise<string> {
                    return AuthService.accessToken;
                }
            })
            .build();

        await this.ensureConnected();
        TodoHubService.newConnectionStartedSub.next();
    }

    private static newConnectionStartedSub: Subscriber<void>;
    public static readonly newConnectionStarted = new Observable<void>(subscriber => {
        TodoHubService.newConnectionStartedSub = subscriber;
    });

    public async removeConnection() {
        await this.hubConnection.stop();
        this.hubConnection = null;
    }

    constructor(private readonly auth: AuthService) {
        this.auth.loggedIn.subscribe(() => {
            if (this.hubConnection == null) {
                this.startNewConnection();
            }
        });
        this.auth.loggedOut.subscribe(async () => {
            await this.removeConnection();
        });

        if (!this.auth.isAuthenticated()) return;

        this.startNewConnection();
    }
}
