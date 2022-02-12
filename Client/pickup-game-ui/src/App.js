import React from "react";
import CssBaseline from '@mui/material/CssBaseline';
import {
    BrowserRouter,
    Routes,
    Route
  } from "react-router-dom";
import Events from "./routes/Events"  
import EventForm from "./routes/EventForm";


function App() {
  return (

    <BrowserRouter>
    <CssBaseline />
    <Routes>
      <Route path="/" element={<Events />} />
      <Route path="create-event" element={ <EventForm />} />
    </Routes>
   </BrowserRouter>
  )
}

export default App;
