import { Component, OnInit } from '@angular/core';
import { BoshClient, $build } from "xmpp-bosh-client/browser";  

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  nick = '';
  message = '';
  messages: string[] = [];

  constructor() { }

  ngOnInit() {
    const USERNAME = "admin@EC2AMAZ-GUL2RT9";
    const PASSWORD = "1q2w3e4r";
    const URL = "http://54.72.144.157:5443/http-bind/";
     
        const client = new BoshClient(USERNAME, PASSWORD, URL);
     
        client.on("error", (e) => {
            console.log("Error event");
            console.log(e);
        });
        client.on("online", () => {
            client.send($build("presence", null, $build('Show', null, 'Chat')));
            console.log("Connected successfully");
        });
        
        client.on("ping", () => {
            console.log(`Ping received at ${new Date()}`);
        });
        
        client.on("stanza", (stanza) => {
            console.log(`Stanza received at ${new Date()}`);
            console.log(stanza);
        });
     
        client.on("offline", () => {
            console.log("Disconnected/Offline");
        });
     
        client.connect();;

  }

}
