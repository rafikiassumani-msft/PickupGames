using PickUpGames.Models;
using PickUpGames.Models.Mappers;

namespace Models.Mappers;


public static class ParticipantMapper {
   
   public static List<ParticipantDTO> MapParticipants(IEnumerable<Participant> participants) {
       
       var mappedParticipants = new List<ParticipantDTO>();

      if(participants is not null && participants.Count() > 0 ) {
            foreach(var participant in participants) {
                var mappedParticipant = MapParticipant(participant);
                mappedParticipants.Add(mappedParticipant);  
            }
      }

       return mappedParticipants;
   }

    public static ParticipantDTO MapParticipant(Participant participant)
    {
        return new ParticipantDTO {
            ParticipantId = participant.ParticipantId,
            User = UserMapper.MapUserWithNoEvent(participant.User),
            Status = participant.Status,
            EventId = participant.EventId,
            CreatedAt = participant.CreatedAt,
            LastUpdatedAt = participant.LastUpdatedAt,
        };
    }
}