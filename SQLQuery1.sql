USE Labs;
SELECT 
    Email,
    LEN(Avatar) as AvatarSize,
    MimeType,
    EmailConfirmed
FROM AspNetUsers 
WHERE Email = 'user3@mail.ru';