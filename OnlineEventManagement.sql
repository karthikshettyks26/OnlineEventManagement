--Create Database
CREATE DATABASE EventManagement

--Create Tables
USE EventManagement

--Create Table Event
CREATE TABLE Event(
	Id UNIQUEIDENTIFIER PRIMARY KEY Default NEWID() NOT NULL,
	Title VARCHAR(100) NOT NULL,
	Description VARCHAR(100) NULL,
	CreatedBy UNIQUEIDENTIFIER NOT NULL,
	CreatedOn DATETIME Default GETDATE() NOT NULL,
	EventDate DATETIME NOT NULL,
	MaxParticipants Int Default 0 NOT NULL
)

--Create Table EventRegister
CREATE TABLE EventRegister
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedOn DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Event(Id)
);

--Create Table Feedback
CREATE TABLE Feedback
(
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
    Rating INT NOT NULL,
    Comment NVARCHAR(500) NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedOn DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Event(Id)
);


--Store Procedures
-- To Add/Insert Event
CREATE PROCEDURE sp_AddEvent
    @Title NVARCHAR(100),
    @Description NVARCHAR(100) = NULL,
    @CreatedBy UNIQUEIDENTIFIER,
    @CreatedOn DATETIME = GETDATE,
    @EventDate DATETIME,
    @MaxParticipants INT = 0,
    @NewEventId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Generate the new ID
    SET @NewEventId = NEWID();

    -- Insert the record
    INSERT INTO Event (Id, Title, Description, CreatedBy, CreatedOn, EventDate, MaxParticipants)
    VALUES (@NewEventId, @Title, @Description, @CreatedBy, @CreatedOn, @EventDate, @MaxParticipants);

    -- Optionally, return the new ID to confirm insertion
    SELECT @NewEventId AS NewEventId;
END;

--Get all Events
CREATE PROCEDURE sp_GetAllEvents
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Title,
        Description,
        CreatedBy,
        CreatedOn,
        EventDate,
        MaxParticipants
    FROM 
        Event
    ORDER BY 
        EventDate ASC; 
END;

--Get Event By Id
CREATE PROCEDURE sp_GetEventById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Title,
        Description,
        CreatedBy,
        CreatedOn,
        EventDate,
        MaxParticipants
    FROM 
        Event
    WHERE
        Id = @Id;
END;

--Update event
CREATE PROCEDURE sp_UpdateEvent
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(100),
    @Description NVARCHAR(100) = NULL,
    @EventDate DATETIME,
    @MaxParticipants INT
AS
BEGIN

    UPDATE Event
    SET 
        Title = @Title,
        Description = @Description,
        EventDate = @EventDate,
        MaxParticipants = @MaxParticipants
    WHERE 
        Id = @Id;
END;

--Delete event
CREATE PROCEDURE sp_DeleteEvent
    @Id UNIQUEIDENTIFIER
AS
BEGIN

    DELETE FROM Event
    WHERE Id = @Id;
END;


--Add Event Register
CREATE PROCEDURE sp_AddEventRegister
    @EventId UNIQUEIDENTIFIER,
    @CreatedBy UNIQUEIDENTIFIER,
    @NewRegisterId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentParticipants INT;

    -- Get the current number of participants for the event
    SELECT @CurrentParticipants = COUNT(*)
    FROM EventRegister
    WHERE EventId = @EventId;

    -- Check if the current participants are less than MaxParticipants
    IF @CurrentParticipants < (SELECT MaxParticipants FROM Event WHERE Id = @EventId)
    BEGIN
        -- Insert the new event registration
        SET @NewRegisterId = NEWID();

        INSERT INTO EventRegister (Id, EventId, CreatedBy, CreatedOn)
        VALUES (@NewRegisterId, @EventId, @CreatedBy, GETDATE());

        -- Return the new register ID
        SELECT @NewRegisterId AS RegisterId;
    END
    ELSE
    BEGIN
        -- If max participants reached, return NULL or raise an error
        SET @NewRegisterId = NULL;
    END
END;


 CREATE PROCEDURE sp_AddFeedback
(
    @Rating INT,
    @Comment NVARCHAR(1000),
    @EventId UNIQUEIDENTIFIER,
    @CreatedBy UNIQUEIDENTIFIER,
	@NewFeedbackId UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
    -- Check if the user is registered for the event (Assuming you have a UserEvents table)
    IF EXISTS (SELECT 1 FROM EventRegister WHERE EventId = @EventId AND CreatedBy = @CreatedBy)
    BEGIN
        INSERT INTO Feedback (Id, Rating, Comment, EventId, CreatedBy, CreatedOn)
        VALUES (NEWID(), @Rating, @Comment, @EventId, @CreatedBy, GetDate());

        
        SELECT @NewFeedbackId AS FeedbackId;
    END
    ELSE
    BEGIN
        SET @NewFeedbackId = null
    END
END








