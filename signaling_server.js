/*******************************************************************
*	Function Name	: webRTC signaling server
*	Description 	: Server which handles the offer and answer request 
********************************************************************/
var WebSocketServer = require('ws').Server;
var wss = new WebSocketServer({ port: 8886 }); // Each PC client has 01 unique port

/* Connection details */
var pc_conns = {}
var and_conns = {}

/* Client information */
var pc_info = new Map();
var and_info = new Map();

/* Signaling server listen for connections */
wss.on('listening', function () {
	console.log(`Server started with port ${this.address().port}`);
});

/* Callback function for connection */
wss.on('connection', function (connection, req) {
	/* Sucessful connection */
	console.log("Client has connected");

	/* Actions to do when client send messages */
	connection.on('message', function (message) {
		
		var isjsonstring = checkIsJson(message); /* messages in Json format */
		
		if(isjsonstring == true)
		{
			/* Parse the messages from client */
			var data = JSON.parse(message);	

			switch (data.type) {
				/* ANDROID CLIENT REQUEST HANDLING */
				/* Connection request to server*/
				case "android_connect_server":
					/* Store the connection details */
					if (and_conns[data.mac]) {
						/* Already same phone in the server */
						sendTo(connection, { type: "server_connect", success: false });
						console.log("Andoir client connection fail");
					}
					else {
						connection.name = data.mac;
						and_conns[data.mac] = connection;

						and_info.set(
							data.mac,
							{
								"name": data.name,
							}
						)
						
						pc = pc_info.get(data.pc_ip)

						if (pc != null) {
							sendTo(connection, { type: "server_connect", success: true, pc_name: pc.name, pc_os: pc.os });
						}
						else
						{
							sendTo(connection, { type: "server_connect", success: true, pc_name: "No PC name", pc_os: "No OS" });
						}
					
						console.log("Android Connection sucess");
						console.log(and_info);
					}

					break;
				
				/* Connection request to pc*/
				case "android_connect_pc":
					var pc_conn = pc_conns[data.pc_ip];

					// console.log(data.pc_ip);
					// console.log(pc_conn);

					
					// console.log("android_connect_pc:");
					// console.log(pc_conn);
					
					var mac = connection.name;

					console.log("Android client send connection request");

					if (pc_conn != null) {
						sendTo(pc_conn, { type: "server_android_connect", mac: mac, name: and_info.get(mac).name });
					}
					else {
						sendTo(connection, { type: "server_no_pc", success: false });
						console.log("No Pc client in server");
					}

					break;	

				/* When pc accept the offer */
				case "pc_accept":
					var android_conn = and_conns[data.mac];
					

					// console.log(connection.name);
					var pc = pc_info.get(connection.name);

					if (android_conn != null) {
						sendTo(android_conn, { type: "server_pc_accept", computer_name:pc.name, OS:pc.os });
						console.log("Pc client accept connection request");
					}
					
					break;

				/* When pc reject the offer */
				case "pc_reject":
					var android_conn = and_conns[data.mac];

					if (android_conn != null) {
						sendTo(android_conn, { type: "server_pc_reject" });
						console.log("Pc client reject connection request");
					}
					
					break;
	
				/* WebRTC offer request from android client*/
				case "android_offer":
					var pc_conn = pc_conns[data.pc_ip];
					
					if (pc_conn != null) {
						sendTo(pc_conn, { type: "server_offer", offer: data.offer, mac: connection.name });
						console.log("Android client send webrtc offer");

						console.log("Android offer:");
						console.log(data.offer);
					}
					else {
						sendTo(connection, { type: "server_no_pc", success: false });
						console.log("No Pc client in server");
					}
	
					break;
	
				/* WebRTC answer request from pc client*/
				case "pc_answer":
					var and_conn = and_conns[data.mac];
					if (and_conn != null) {
						/* Send the answer back to android client */
						sendTo(and_conn, { type: "server_pc_answer", answer: data.answer });
						console.log("Pc client send webrtc answer");

						console.log("PC answer")
						console.log(data.answer);
					}
	
					break;
	
				/* Candidate request from android client*/
				case "android_candidate":
					var pc_conn = pc_conns[data.pc_ip];

					// console.log("android_candidate "+ pc_conn);

					if (pc_conn != null) {
						// console.log("Sever:")
						// console.log(data.sdpMid)
						// console.log(data.sdpMLineIndex)
						// console.log(data.candidate)

						sendTo(pc_conn, { type: "server_candidate", sdpMid: data.sdpMid, sdpMLineIndex: data.sdpMLineIndex, candidate: data.candidate });
						console.log("Android client send webrtc candidate");

						console.log("Android candidate:");
						console.log(data.sdpMid);
						console.log(data.sdpMLineIndex);
						console.log(data.candidate);
					}
					else	
					{
						console.log("No pc");
					}
					
					break;
	
				/* Disconnect request from android client*/
				case "android_disconnect_pc":
					/* Get client details */
					var pc_conn = pc_conns[data.pc_ip];
					if (pc_conn != null) {
						sendTo(pc_conn, { type: "server_android_disconnect", mac: connection.name, last_active: new Date() });
						console.log("Android client disconnect Pc");
					}
	
					break;
				
				/* PC CLIENT REQUEST HANDLING */
				/* Connection request to server */
				case "pc_connect_server":
					connection.name = data.ip;
					pc_conns[data.ip] = connection;

					pc_info.set(
						data.ip,
						{
							"name": data.name,
							"os": data.os,
						}
					)

					sendTo(connection, { type: "server_connect", success: true });
					console.log("Pc client connection sucess");
					console.log(pc_info);

					break;
				
				/* Candidate request*/
				case "pc_candidate":
					var and_conn = and_conns[data.mac];
					if (and_conn != null) {
						// console.log("Sever:");
						// console.log(data);
						
						sendTo(and_conn, { type: "server_candidate", candidate: data.candidate });
						console.log("Pc client send webrtc candidate");

						console.log("PC candidate:");
						console.log(data.candidate);
					}
					break;

				/* Disconnect request from pc client*/
				case "pc_disconnect_android":
					/* Get client details */
					var and_conn = and_conns[data.mac];

					if (and_conn != null) {
						sendTo(and_conn, { type: "server_pc_disconnect", pc_ip: connection.name, last_active: new Date() });
						console.log("Pc client disconnect android");
					}
	
					break;
	
				/* default */
				default:
					sendTo(connection, { type: "server_error", message: "Unrecognized command: " + data.type });
					console.log("Unrecognized server message");
					break;
			}
		}
		else
		{
			/* ping from client, so repond with pong to get server is alive.*/
			if(message == "clientping")
			{
				sendTo(connection, { type: "server_pong", name: "pong" });
			}
		}
	});

	/* Actions to do when Android client close application */
	connection.on('close', function () {
		console.log("Client closed");
		if (connection.name) {
			
			for (var pc in pc_conns) {
				sendTo(pc_conns[pc], { type: "server_android_close", mac: connection.name, last_active: new Date() });
			}

			delete and_conns[connection.name];
		}
	});
});

/* Check the message is JSON or not */
function checkIsJson(str) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}

/* function to remove item from array by value*/
function remove(value, array) {
	var index = array.indexOf(value);
	if (index !== -1) {
		array.splice(index, 1);
		
	}
}

/* function to send the message to clients*/
function sendTo(conn, message) {
	conn.send(JSON.stringify(message));
}