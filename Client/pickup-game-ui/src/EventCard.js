import * as React from 'react';
import { styled } from '@mui/material/styles';
import Card from '@mui/material/Card';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import CardContent from '@mui/material/CardContent';
import CardActions from '@mui/material/CardActions';
import Avatar from '@mui/material/Avatar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import { red } from '@mui/material/colors';
import FavoriteIcon from '@mui/icons-material/Favorite';
import ShareIcon from '@mui/icons-material/Share';
import MoreVertIcon from '@mui/icons-material/MoreVert';
import Chip from "@mui/material/Chip"
import LocationOnIcon from '@mui/icons-material/LocationOn';
import SportsScoreIcon from '@mui/icons-material/SportsScore';
import EventIcon from '@mui/icons-material/Event';
import { Container, Grid } from '@mui/material';

export default function EventCard(props) {
  const [expanded, setExpanded] = React.useState(false);

  const handleExpandClick = () => {
    setExpanded(!expanded);
  };

  return (
    <Card>
      <CardHeader
        avatar={
          <Avatar sx={{ bgcolor: red[500] }} aria-label="recipe">
            SE
          </Avatar>
        }
        action={
          <IconButton aria-label="settings">
            <MoreVertIcon />
          </IconButton>
        }
        title={`Sport Event - ${props.eventId}`}
        subheader={props.startDate}
      />
      <CardMedia
        component="img"
        height="194"
        image="sport-event.jpg"
        alt="Pick up events"
      />
      <CardContent>
        <Typography variant="body2" color="text.secondary">
           {props.description}
        </Typography>
      </CardContent>
      <CardActions disableSpacing>
        <IconButton aria-label="add to favorites">
          <FavoriteIcon />
        </IconButton>
        <IconButton aria-label="share">
          <ShareIcon />
        </IconButton>
        <Container maxWidth="sm">
         <Grid container spacing={2}>
            <Grid item xs={4}>  
             <Chip icon={<LocationOnIcon />} label={props.location} variant="outlined" />
            </Grid> 
            <Grid item xs={4}>  
              <Chip icon={<EventIcon />} label={props.eventStatus} color="success" />
            </Grid>
            <Grid item xs={4}> 
              <Chip icon={<SportsScoreIcon />} label={props.eventType} variant="outlined" />
            </Grid>
         </Grid>
        </Container>
      </CardActions>
    </Card>
  );
}
