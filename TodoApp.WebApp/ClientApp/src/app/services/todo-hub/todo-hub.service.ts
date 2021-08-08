import {Injectable} from '@angular/core';
import {HttpTransportType, HubConnection, HubConnectionBuilder, HubConnectionState} from "@microsoft/signalr";

@Injectable({
    providedIn: 'root'
})
export class TodoHubService {
    public hubConnection: HubConnection;

    public async ensureConnected() {
        if (this.hubConnection.state == HubConnectionState.Disconnected) {
            await this.hubConnection.start();
        }
    }

    constructor() {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl("/hubs/todo", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .build();
    }
}
