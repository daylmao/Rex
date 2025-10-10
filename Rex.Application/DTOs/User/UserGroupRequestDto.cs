using System.ComponentModel.DataAnnotations;

namespace Rex.Application.DTOs;

public record UserGroupRequestDto(
    string FirstName,
    string LastName,
    string ProfilePicture,
    string Status,
    
    TimeSpan TimeSinceRequested 
    );