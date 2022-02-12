//import * as signalR from "@microsoft/signalr"
let signalR = require("@microsoft/signalr")


let signalRConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7287/eventMessage", {withCredentials: false})
    .configureLogging(signalR.LogLevel.Trace)
    .build();

export default signalRConnection;