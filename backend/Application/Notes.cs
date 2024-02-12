using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Models;
using NotesApi.Controllers.Dto;
using System.Linq;

namespace NotesApi.Application
{
    public class Notes
    {
        private readonly ApiDbContext _context;

        public Notes(ApiDbContext context)
        {
            _context = context;
        }
        public GenericMessage<DtoGetNotes> GetById(Guid NoteId)
        {
            Note note = _context.Notes?.FirstOrDefault(n => n.Id.Equals(NoteId)) ?? new Note();
            List<TagNote> tagNotes = _context.TagsNotes?.Where(n => n.NoteId.Equals(NoteId)).ToList() ?? new List<TagNote>();
            List<string> tags = new();

            foreach(TagNote tagNote in tagNotes)
            {
                tags.Add(tagNote.Tag);
            }

            DtoGetNotes dtoGetNotes = new DtoGetNotes();
            dtoGetNotes.Note = note;
            dtoGetNotes.Tags = tags;

            if (note.Id.Equals(Guid.Empty))
            {
                return new GenericMessage<DtoGetNotes>(dtoGetNotes, "Note not found.");
            }
            else
            {
                return new GenericMessage<DtoGetNotes>(dtoGetNotes, "Note found succesfully.");
            }
        }

        public List<DtoGetNotes> GetByUserIdTags (Guid UserId, bool IsDeleted, bool Active, List<string> Tags)
        {
            List<TagNote> tagNotes0 = new List<TagNote>();
            foreach(string Tag in Tags)
            {
                tagNotes0.AddRange(_context.TagsNotes?.Where(a => a.Tag.Equals(Tag)).ToList()??new List<TagNote>());
            }
            List<Note> notes = _context.Notes?
                .Where(a => a.UserId.Equals(UserId)
                && a.IsDeleted.Equals(IsDeleted)
                && a.Active.Equals(Active)
                ).ToList() ?? new List<Note>();

            List<DtoGetNotes> dtoGetNotes = new List<DtoGetNotes>();

            foreach (Note note in notes)
            {

                DtoGetNotes dtoGetNoteHelper = new();
                dtoGetNoteHelper.Note = note;

                List<TagNote> tagNotes = _context.TagsNotes?.Where(a => a.NoteId.Equals(note.Id)).ToList() ?? new List<TagNote>();
                List<string> tags = new();
                foreach (var tagNote in tagNotes)
                {
                    tags.Add(tagNote.Tag);
                }
                dtoGetNoteHelper.Tags = tags;
                dtoGetNotes.Add(dtoGetNoteHelper);
            }

            List<DtoGetNotes> dtoGetNotesResponse = new List<DtoGetNotes>();

            foreach (DtoGetNotes dtoGetNote in dtoGetNotes)
            {
                TagNote tagNote = tagNotes0.Where(a=>a.NoteId.Equals(dtoGetNote.Note.Id)).FirstOrDefault()??new TagNote();

                if (!tagNote.Id.Equals(Guid.Empty))
                {
                    dtoGetNotesResponse.Add(dtoGetNote);
                }
            }

            return dtoGetNotesResponse;
        }

        public List<DtoGetNotes> GetByUserId(Guid UserId, bool IsDeleted, bool Active)
        {
            List<Note> notes = _context.Notes?
                .Where(a => a.UserId.Equals(UserId)
                && a.IsDeleted.Equals(IsDeleted)
                && a.Active.Equals(Active)
                ).ToList() ?? new List<Note>();

            List<DtoGetNotes> dtoGetNotes= new List<DtoGetNotes>();

            foreach (Note note in notes)
            {

                DtoGetNotes dtoGetNoteHelper = new();
                dtoGetNoteHelper.Note = note;

                List<TagNote> tagNotes = _context.TagsNotes?.Where(a => a.NoteId.Equals(note.Id)).ToList()??new List<TagNote>();
                List<string> tags = new();
                foreach (var tagNote in tagNotes)
                {
                    tags.Add(tagNote.Tag);
                }
                dtoGetNoteHelper.Tags = tags;
                dtoGetNotes.Add(dtoGetNoteHelper);
            }
            return dtoGetNotes;
        }

        public GenericMessage<Note> Create(Note note, List<string> tags)
        {
            if (note == null)
            {
                return new GenericMessage<Note>(new Note(), "You have sent an empty note");
            }
            else
            {

                if (note.Id.Equals(Guid.Empty))
                {
                    note.Id = Guid.NewGuid();
                }
                else
                {
                    return new GenericMessage<Note>(note, "Guid must be in empty format (00000000-0000-0000-0000-000000000000).");
                }

                User user = _context.Users?.FirstOrDefault(a=>a.Id.Equals(note.UserId))??new User();

                if (user.Id.Equals(Guid.Empty))
                {
                    return new GenericMessage<Note>(note, "The user is not valid");
                }

                Note existnote = _context.Notes?.FirstOrDefault(u => u.Name.Equals(note.Name) && u.UserId.Equals(note.UserId)) ?? new Note();

                if (existnote.Id.Equals(Guid.Empty))
                {
                    note.User = user;
                    _context.Notes?.Add(note);
                    _context.SaveChanges();
                    Note creatednote = _context.Notes?.FirstOrDefault(a => a.Id.Equals(note.Id)) ?? throw new Exception("Note not created.");

                    if (tags is not null)
                    {
                        foreach (string tag in tags)
                        {
                            TagNote tagNote = new TagNote();
                            tagNote.Tag = tag;
                            tagNote.NoteId = note.Id;
                            tagNote.CreateDate = DateTime.Now;
                            tagNote.Id = Guid.NewGuid();
                            _context.TagsNotes?.Add(tagNote);
                            _context.SaveChanges();
                        }
                    }

                    return new GenericMessage<Note>(creatednote, "Note created correctly.");
                }
                else
                {
                    return new GenericMessage<Note>(new Note(), "Note already exist");
                }
            }
        }

        public GenericMessage<Note> Update(Guid Id, Note note, List<string> tags)
        {
            if (note == null)
            {
                return new GenericMessage<Note>(new Note(), "You have sent an empty note");
            }
            else
            {
                Guid exist = _context.Notes?.FirstOrDefault(n => n.Id.Equals(Id)).Id ?? Guid.Empty;
                if (exist.Equals(Guid.Empty))
                {
                    return new GenericMessage<Note>(note, "Note not found.");
                }
                else
                {
                    

                    if (tags.Count() > 0 )
                    {
                        foreach (string tag in tags)
                        {
                            TagNote existingTag = _context.TagsNotes?.FirstOrDefault(a => a.NoteId.Equals(Id) && a.Tag.Equals(tag)) ?? new TagNote();
                            if (existingTag.Tag.Equals(string.Empty))
                            {
                                TagNote tagNote = new TagNote();
                                tagNote.Tag = tag;
                                tagNote.NoteId = Id;
                                tagNote.CreateDate = DateTime.Now;
                                _context.TagsNotes?.Add(tagNote);
                            }
                        }
                    }
                    List<TagNote> existingTags = _context.TagsNotes?.Where(a => a.NoteId.Equals(Id)).ToList() ?? new List<TagNote>();
                    if (existingTags.Count > 0)
                    {
                        foreach (TagNote tagNote in existingTags)
                        {
                            if (!tags.Contains(tagNote.Tag))
                            {
                                _context.TagsNotes?.Remove(tagNote);
                            }
                        }
                    }

                    Note oldNote = _context.Notes?.FirstOrDefault(n => n.Id.Equals(Id)) ?? new Note();

                    Note existOtherNote = _context.Notes?.FirstOrDefault(n => n.Name.Equals(note.Name) && n.UserId.Equals(oldNote.UserId) && n.Id != Id) ?? new Note();

                    if (!existOtherNote.Id.Equals(Guid.Empty))
                    {
                        return new GenericMessage<Note>(note, "There is already another note with the same name");
                    }
                    oldNote.Name = note.Name;
                    oldNote.Content = note.Content;

                    _context.Notes?.Update(oldNote);
                    _context.SaveChanges();
                    return new GenericMessage<Note>(oldNote, "Note Updated.");
                }
            }
        }

        public GenericMessage<Note> Delete(Guid NoteId)
        {
            if (NoteId.Equals(Guid.Empty))
            {
                return new GenericMessage<Note>(new Note(), "The Guid is in empty format.");
            }
            else
            {
                Note note = _context.Notes.FirstOrDefault(a => a.Id.Equals(NoteId)) ?? new Note();
                if (note.Id.Equals(Guid.Empty))
                {
                    return new GenericMessage<Note>(note, "The note does not exist.");
                }
                else
                {
                    if(note.IsDeleted)
                    {
                        return new GenericMessage<Note>(note, "The note is already deleted.");
                    }
                    note.IsDeleted = true;
                    _context.Notes.Update(note);
                    _context.SaveChanges();
                    
                    Note updatedNote = _context.Notes.FirstOrDefault(a=>a.Id.Equals(note.Id))?? new Note();
                    if (updatedNote.IsDeleted.Equals(false))
                    {
                        return new GenericMessage<Note>(note, "Something went wrong. The note has not been deleted.");
                    }
                    else
                    {
                        return new GenericMessage<Note>(note, "The note has been deleted succesfully.");
                    }
                }
            }
        }
        public GenericMessage<Note> Recover(Guid NoteId)
        {
            if (NoteId.Equals(Guid.Empty))
            {
                return new GenericMessage<Note>(new Note(), "The Guid is in empty format.");
            }
            else
            {
                Note note = _context.Notes.FirstOrDefault(a => a.Id.Equals(NoteId)) ?? new Note();
                if (note.Id.Equals(Guid.Empty))
                {
                    return new GenericMessage<Note>(note, "The note does not exist.");
                }
                else
                {
                    if (!note.IsDeleted)
                    {
                        return new GenericMessage<Note>(note, "The note is not deleted.");
                    }
                    note.IsDeleted = false;
                    _context.Notes.Update(note);
                    _context.SaveChanges();

                    Note updatedNote = _context.Notes.FirstOrDefault(a => a.Id.Equals(note.Id)) ?? new Note();
                    if (updatedNote.IsDeleted.Equals(true))
                    {
                        return new GenericMessage<Note>(note, "Something went wrong. The note has not been recovered.");
                    }
                    else
                    {
                        return new GenericMessage<Note>(note, "The note has been deleted succesfully.");
                    }
                }
            }
        }
        public GenericMessage<Note> ToArchive(Guid NoteId)
        {
            if (NoteId.Equals(Guid.Empty))
            {
                return new GenericMessage<Note>(new Note(), "The Guid is in empty format.");
            }
            else
            {
                Note note = _context.Notes.FirstOrDefault(a => a.Id.Equals(NoteId)) ?? new Note();
                if (note.Id.Equals(Guid.Empty))
                {
                    return new GenericMessage<Note>(note, "The note does not exist.");
                }
                else
                {
                    if (!note.Active)
                    {
                        return new GenericMessage<Note>(note, "The note is already archived.");
                    }
                    note.Active = false;
                    _context.Notes.Update(note);
                    _context.SaveChanges();

                    Note updatedNote = _context.Notes.FirstOrDefault(a => a.Id.Equals(note.Id)) ?? new Note();
                    if (updatedNote.Active.Equals(true))
                    {
                        return new GenericMessage<Note>(note, "Something went wrong. The note has not been archived.");
                    }
                    else
                    {
                        return new GenericMessage<Note>(note, "The note has been archived succesfully.");
                    }
                }
            }
        }

        public GenericMessage<Note> ToActive(Guid NoteId)
        {
            if (NoteId.Equals(Guid.Empty))
            {
                return new GenericMessage<Note>(new Note(), "The Guid is in empty format.");
            }
            else
            {
                Note note = _context.Notes.FirstOrDefault(a => a.Id.Equals(NoteId)) ?? new Note();
                if (note.Id.Equals(Guid.Empty))
                {
                    return new GenericMessage<Note>(note, "The note does not exist.");
                }
                else
                {
                    if (note.Active)
                    {
                        return new GenericMessage<Note>(note, "The note is already active.");
                    }
                    note.Active = true;
                    _context.Notes.Update(note);
                    _context.SaveChanges();

                    Note updatedNote = _context.Notes.FirstOrDefault(a => a.Id.Equals(note.Id)) ?? new Note();
                    if (updatedNote.Active.Equals(false))
                    {
                        return new GenericMessage<Note>(note, "Something went wrong. The note has not been moved to active.");
                    }
                    else
                    {
                        return new GenericMessage<Note>(note, "The note has been moved to active succesfully.");
                    }
                }
            }
        }
    }
}
