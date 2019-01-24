import { Component, OnInit } from '@angular/core';
import { BoshClient, $build } from "xmpp-bosh-client/browser";
import { NotificationsService } from '../core/notifications.service';
import { SnackBarService } from "../core/snack-bar.service";

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  notifications: string[] = [];
  username = "";
  password = "";
  client: BoshClient = null;

  constructor(private notify: NotificationsService, private snackBar: SnackBarService) { }

  ngOnInit() {
    this.notify.getNotificationsAuth()
      .subscribe(notificationsAuth => {
        this.username = notificationsAuth.Username;
        this.password = notificationsAuth.Password;
        this.client = new BoshClient(`${this.username}${DOMAIN}`, this.password, URL);
        this.setEvents(this.client);
        this.client.connect();
      },
        (err) => {
          this.snackBar.openSnackBar(err, "", 10000);
        }
      );

    const URL = "http://54.72.144.157:5443/http-bind/";
    const DOMAIN = "@EC2AMAZ-GUL2RT9";



  }
  setEvents(client: BoshClient): any {
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
      if (stanza.name === "message") {
        let message = stanza.children.find(elm => elm.name==='body').children[0];

        this.notifications.unshift(message.toString());
      }
    });

    client.on("offline", () => {
      console.log("Disconnected/Offline");
    });
  }
}
