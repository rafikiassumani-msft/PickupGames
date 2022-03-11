
using PickUpGames.Models;
using PickUpGames.Models.Mappers;

namespace Models.Mappers;


public static class EventMapper {
    
    public static List<EventDTO> MapEvents(IEnumerable<Event> allEvents) {
        
        var mappedEvents = new List<EventDTO>();

       if (allEvents is not null) {
            foreach(var sportEvent in allEvents) {
                var mappedEvent = MapEventWithOwner(sportEvent);
                mappedEvents.Add(mappedEvent);
            }
       }

        return mappedEvents;

    }

    public static EventDTO MapEvent(Event sportEvent)
    {
       return new EventDTO {
            EventId = sportEvent.EventId,
            Title = sportEvent.Title,
            Description = sportEvent.Description,
            StartDate = sportEvent.StartDate,
            StartTime = sportEvent.StartTime,
            Location = MapAddressToResponseDTO(sportEvent.Address),
            MaxNumberOfParticipants = sportEvent.MaxNumberOfParticipants,
            EventPrivacy = sportEvent.EventPrivacy.ToString(),
            EventStatus = sportEvent.EventStatus.ToString(),
            EventType = sportEvent.EventType.ToString(),
            Participants = ParticipantMapper.MapParticipants(sportEvent.Participants),
            CreatedAt = sportEvent.CreatedAt,
            LastUpdatedAt = sportEvent.LastUpdatedAt
        };
    }

    
    public static Event MapEventRequestDTO(EventRequestDTO sportEvent)
    {
       return new Event {
            Title = sportEvent.Title,
            Description = sportEvent.Description,
            StartDate = sportEvent.StartDate,
            StartTime = sportEvent.StartTime,
            Address = MapAddressRequestDTO(sportEvent.Location),
            MaxNumberOfParticipants = sportEvent.MaxNumberOfParticipants,
            EventPrivacy = (EventPrivacy) sportEvent.EventPrivacy,
            EventStatus = (EventStatus) sportEvent.EventStatus,
            EventType = (EventType) sportEvent.EventType,
        };
    }

   public static EventDTO MapEventWithOwner(Event sportEvent)
    {
       return new EventDTO {
            EventId = sportEvent.EventId,
            Title = sportEvent.Title,
            Description = sportEvent.Description,
            StartDate = sportEvent.StartDate,
            StartTime = sportEvent.StartTime,
            Location = MapAddressToResponseDTO(sportEvent.Address),
            MaxNumberOfParticipants = sportEvent.MaxNumberOfParticipants,
            EventPrivacy = sportEvent.EventPrivacy.ToString(),
            EventStatus = sportEvent.EventStatus.ToString(),
            EventType = sportEvent.EventType.ToString(),
            Participants = ParticipantMapper.MapParticipants(sportEvent.Participants),
            Owner = UserMapper.MapUserWithNoEvent(sportEvent.User),
            CreatedAt = sportEvent.CreatedAt,
            LastUpdatedAt = sportEvent.LastUpdatedAt
        };
    }

    public static Address MapAddressRequestDTO(AddressRequestDTO addressRequestDTO) {

        return new Address {
            StreetAddress = addressRequestDTO.StreetAddress,
            StreetAddress2 = addressRequestDTO.StreetAddress2,
            City = addressRequestDTO.City,
            State = addressRequestDTO.State,
            PostalCode = addressRequestDTO.PostalCode,
            Country = addressRequestDTO.Country
        };
    }

    public static AddressRequestDTO MapAddressToResponseDTO(Address address) {

        return new () {
            StreetAddress = address.StreetAddress,
            StreetAddress2 = address.StreetAddress2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }
}