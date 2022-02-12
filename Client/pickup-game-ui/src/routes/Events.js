import React from "react";
import CssBaseline from '@mui/material/CssBaseline';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import signalRConnection from "../signalR";
import EventCard from "../EventCard";
import {Typography } from "@mui/material";

function Events() {

    const [pickupGameEvents, setPickupGameEvents] = React.useState([]);
    const [signalRConnectionState, SetSignalRConnectionState] = React.useState(null);

    React.useEffect(() => {
        const startConnection = async () => {
            return await connectAsync();
        }
        startConnection();
    }, []);

    React.useEffect(() => {
        signalRConnection.on("ReceiveEventMessages", (eventMessage) => {
            console.log(eventMessage)
            setPickupGameEvents(eventMessage)
        }); 

        return () => {
            console.log("cleaning signalr handlers");
            signalRConnection?.off(); 
        };
    }, [pickupGameEvents]);

    React.useEffect(() => {
       if (signalRConnectionState === "Connected") {
        try {
            signalRConnection.invoke("SendEventMessages", "hello from client");
        } catch(err) {
           console.log("Cannot send message")
           console.error(err);
        }
      }     
    }, [signalRConnectionState])


    const connectAsync = async () => {
        try {
            await signalRConnection.start();
            console.log(`We are : ${signalRConnection.state}`)
            SetSignalRConnectionState(signalRConnection.state)
        } catch (err) {
            console.error(err);
        }
    }
 

  return (

    <React.Fragment>
         <CssBaseline />           
         <Container disableGutters maxWidth="sm" component="main" sx={{ pt: 8, pb: 6 }}>
                <Typography
                component="h1"
                variant="h2"
                align="center"
                color="text.primary"
                gutterBottom
                >
                List of all sport events
                </Typography>
                <Typography variant="h5" align="center" color="text.secondary" component="p">
                    List of pick up games in your city!
                </Typography>
          </Container>

          <Container disableGutters maxWidth="sm" component="main">
          <Grid container spacing={5} alignItems="flex-end">

                {
                    pickupGameEvents.length !== 0 && pickupGameEvents.map(eventMessage => 
                      <Grid item sm={12}>
                        <EventCard  key={eventMessage.eventId} {...eventMessage} />
                      </Grid>             
                    )
                }
           </Grid>
          </Container>
   </React.Fragment>
  )
}

export default Events;
