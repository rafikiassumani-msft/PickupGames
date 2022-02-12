import React from "react"
import {
    Container, Grid, Box, FormControl, 
    TextField, Typography, MenuItem, 
    Select, Button, InputLabel, TextareaAutosize 
} from "@mui/material";

import DatePicker from '@mui/lab/DatePicker';
import DateAdapter from '@mui/lab/AdapterMoment';
import LocalizationProvider from '@mui/lab/LocalizationProvider';
import TimePicker from '@mui/lab/TimePicker';
import moment from "moment";


const eventFormReducer = (state, eventData) => {
    return {
        ...state,
        [eventData.name]: eventData.value
    }
}

const EventForm = () => {

    const [formData, setFormData] = React.useReducer(eventFormReducer, {});

    const handleChange = (event) => {
      setFormData({
          name: event.target.name,
          value: event.target.value
      })
    }

    const handleStartDateChange = (newDate) => {
       setFormData({
           "name": "startDate",
           "value": moment(newDate).format("L")
         })
       console.log(formData)
    }

    const handleTimeChange = (newTime) => {
        setFormData({
            "name": "startTime",
            "value": moment(newTime).format()
          })
        console.log(formData)
    }

    const handleSubmit = (event) => {
        event.preventDefault();
        fetch("https://localhost:7287/events", {
            method: "post",
            body: JSON.stringify({...formData, ownerId: 1})
        }).then(response => {
            if(!response.ok) {
              throw new Error(`HTTP Post - Failed to create event - Http status: ${response.status}`)
            }
            return response.json();
        }).catch(err => {
            console.log(err)
        })
    }

    return (
        <React.Fragment>
         <Container disableGutters maxWidth="sm" component="main" sx={{ pt: 8, pb: 6 }}>
                <Typography variant="h5" align="center" color="text.secondary" component="p">
                    Create an event
                </Typography>
          </Container>
  

          <Container disableGutters maxWidth="sm" component="main">
          <Grid container spacing={5} alignItems="flex-end">
          <Box sx={{ display: 'flex', flexWrap: 'wrap' }}
            component="form"
            noValidate
            autoComplete="off"
            onSubmit={handleSubmit}
            >
     
                <FormControl fullWidth>
                    <TextField required name="title" id="outlined-basic" label="Event Name" variant="outlined"  margin="normal" onChange={handleChange} value ={formData.eventName || ''} />
                </FormControl>

                <FormControl fullWidth margin="normal">
                 <InputLabel id="event-type-select-label">Event Type</InputLabel>
                 <Select
                    name="eventType"
                    labelId="event-type-select-label"
                    id="event-type-select"
                    value={formData.eventType || ''}
                    label="eventType"
                    onChange={handleChange}
                 >
                    <MenuItem value={1}>Soccer</MenuItem>
                    <MenuItem value={2}>Baskball</MenuItem>
                    <MenuItem value={3}>DodgeBall</MenuItem>
                </Select>
               </FormControl>

                <FormControl fullWidth>
                    <TextareaAutosize
                        required
                        onChange={handleChange}
                        name="description" 
                        value ={formData.description || ''}
                        aria-label="Event description"
                        minRows={10}
                        margin="normal"
                        placeholder="Event Description"
                        style={{ marginBottom: "20px", marginTop: "10px", padding: 10 }}
                      />
                </FormControl>
            
                <FormControl fullWidth>
                    <LocalizationProvider dateAdapter={DateAdapter}>

                      <Grid container spacing={2}>
                        <Grid item xs={4}> 
                            <DatePicker
    x                           label="Start Date"
                                margin="normal"
                                onChange={handleStartDateChange}
                                value={formData.startDate || null}
                                renderInput={(params) => <TextField name="startDate" {...params} />}
                            />
                       </Grid>   

                      <Grid item xs={4}> 
                       <TimePicker
                        label="Start Time"
                        value={formData.startTime || null}
                        onChange={handleTimeChange}
                        renderInput={(params) => <TextField {...params} />}
                      />
                      </Grid>

                      <Grid item xs={4}> 
                        <TimePicker
                            label="End Time"
                            value={formData.endTime || null}
                            onChange={handleTimeChange}
                            renderInput={(params) => <TextField {...params} />}
                        />
                      </Grid>
                      </Grid>
                    </LocalizationProvider>
                </FormControl>

                <FormControl fullWidth>
                 <TextField required name="streetAddress" id="outlined-basic" label="Street Address" variant="outlined"  margin="normal" onChange={handleChange} value ={formData.streetAddress || ''} />
                </FormControl>

                <FormControl fullWidth>
                 <TextField name="streetAddress2" id="outlined-basic" label="Street Address 2" variant="outlined"  margin="normal" onChange={handleChange} value ={formData.streetAddress2 || ''} />
                </FormControl>

                <Grid container spacing={2}>
                    <Grid item xs={6}>
                     <FormControl fullWidth>
                        <TextField name="city" id="outlined-basic" label="City" variant="outlined"  margin="normal" onChange={handleChange} value ={formData.city || ''} />
                     </FormControl>
                    </Grid>

                    <Grid item xs={6}>
                     <FormControl fullWidth>
                        <TextField name="state" id="outlined-basic" label="State/Province" variant="outlined"  margin="normal" onChange={handleChange} value ={formData.province || ''} />
                     </FormControl>
                    </Grid>
                </Grid>

                <Grid container spacing={2}>
                    <Grid item xs={6}>
                     <FormControl fullWidth>
                        <TextField name="postalCode" id="outlined-basic" label="Zip code/Postal Code" variant="outlined"  margin="normal" onChange={handleChange} value ={formData.postalCode || ''} />
                     </FormControl>
                    </Grid>

                    <Grid item xs={6}>
                     <FormControl fullWidth>
                        <TextField name="country" id="outlined-basic" label="Country" variant="outlined"  margin="normal" onChange={handleChange} value ={formData.country || ''} />
                     </FormControl>
                    </Grid>
                </Grid>

                <FormControl fullWidth margin="normal">
                 <InputLabel id="event-privacy-select-label">Event Privacy</InputLabel>
                 <Select
                    name="eventPrivacy"
                    labelId="event-privacy-select-label"
                    id="event-privacy-select"
                    value={formData.eventPrivacy || ''}
                    label="eventPrivacy"
                    onChange={handleChange}
                 >
                    <MenuItem value={1}>Public</MenuItem>
                    <MenuItem value={2}>Private</MenuItem>
                </Select>
               </FormControl>

            

               <FormControl fullWidth margin="normal">
                 <InputLabel id="event-status-select-label">Event Status</InputLabel>
                 <Select
                    name="eventStatus"
                    labelId="event-status-select-label"
                    id="event-status-select"
                    value={formData.eventStatus || ''}
                    label="eventStatus"
                    onChange={handleChange}
                 >
                    <MenuItem value={1}>Open</MenuItem>
                    <MenuItem value={2}>Close</MenuItem>
                </Select>
               </FormControl>

               <Button type="submit" variant="contained"> Save Event Data</Button>


           </Box>
           </Grid>
          </Container>
   </React.Fragment>
    )
}

export default EventForm