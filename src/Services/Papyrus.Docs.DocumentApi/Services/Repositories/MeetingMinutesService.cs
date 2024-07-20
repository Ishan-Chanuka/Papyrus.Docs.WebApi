namespace Papyrus.Docs.DocumentApi.Services.Repositories
{
    public class MeetingMinutesService(ApplicationDbContext _context) : IMeetingMinutesService
    {
        private string message = string.Empty;
        public async Task<ApiResponse<Guid>> CreateMeetingMinutesAsync(MeetingMinutesDto request)
        {
            if (request == null)
            {
                message = "Request is null";
                return new ApiResponse<Guid>(message: message, statusCode: (int)HttpStatusCode.BadRequest);
            }

            var meetingMinutes = new MeetingMinutes
            {
                Title = request.Title,
                MeetingNo = request.MeetingNo,
                Description = request.Description,
                MeetingDate = request.MeetingDate,
                CreatedUser = request.CreatedUser,
                IsDeleted = false
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.MeetingMinutes.Add(meetingMinutes);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ApiResponse<Guid>
                {
                    StatusCode = (int)HttpStatusCode.Created, // 201
                    Message = "Meeting minutes created successfully",
                    IsSuccess = true,
                    Data = meetingMinutes.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<Guid>(message: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError); // 500
            }
        }

        public async Task<ApiResponse<IEnumerable<MeetingMinutesResponseModel>>> GetMeetingMinutesAsync()
        {
            var meetingMinutes = await _context.MeetingMinutes.Where(mm => mm.IsDeleted == false)
                                                              .Select(res => new MeetingMinutesResponseModel
                                                              {
                                                                  Id = res.Id,
                                                                  Title = res.Title,
                                                                  MeetingNo = res.MeetingNo,
                                                                  Description = res.Description,
                                                                  MeetingDate = res.MeetingDate
                                                              }).ToListAsync();

            if (meetingMinutes.Count == 0)
            {
                message = "Meeting minutes not found";
                return new ApiResponse<IEnumerable<MeetingMinutesResponseModel>>(message: message, statusCode: (int)HttpStatusCode.NotFound); // 404
            }

            return new ApiResponse<IEnumerable<MeetingMinutesResponseModel>>
            {
                StatusCode = (int)HttpStatusCode.OK, // 200
                Message = "Meeting minutes retrieved successfully",
                IsSuccess = true,
                Data = meetingMinutes
            };
        }

        public async Task<ApiResponse<Guid>> UpdateMeetingMinutesAsync(Guid id, MeetingMinutesDto request)
        {
            var meetingMinutes = await _context.MeetingMinutes.FirstOrDefaultAsync(mm => mm.Id == id);

            if (meetingMinutes == null)
            {
                message = "Meeting minutes not found";
                return new ApiResponse<Guid>(message: message, statusCode: (int)HttpStatusCode.NotFound); // 404
            }

            meetingMinutes.Title = request.Title;
            meetingMinutes.MeetingNo = request.MeetingNo;
            meetingMinutes.Description = request.Description;
            meetingMinutes.MeetingDate = request.MeetingDate;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.MeetingMinutes.Update(meetingMinutes);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ApiResponse<Guid>
                {
                    StatusCode = (int)HttpStatusCode.OK, // 200
                    Message = "Meeting minutes updated successfully",
                    IsSuccess = true,
                    Data = meetingMinutes.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<Guid>(message: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError); // 500
            }
        }

        public async Task<ApiResponse<Guid>> DeleteMeetingMinutesAsync(Guid id)
        {
            var meetingMinutes = await _context.MeetingMinutes.FirstOrDefaultAsync(mm => mm.Id == id);

            if (meetingMinutes == null)
            {
                message = "Meeting minutes not found";
                return new ApiResponse<Guid>(message: message, statusCode: (int)HttpStatusCode.NotFound); // 404
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                meetingMinutes.IsDeleted = true;
                _context.MeetingMinutes.Update(meetingMinutes);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ApiResponse<Guid>
                {
                    StatusCode = (int)HttpStatusCode.OK, // 200
                    Message = "Meeting minutes deleted successfully",
                    IsSuccess = true,
                    Data = meetingMinutes.Id
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<Guid>(message: ex.Message, statusCode: (int)HttpStatusCode.InternalServerError); // 500
            }
        }

        public async Task<ApiResponse<MeetingMinutesResponseModel>> GetMeetingMinutesByIdAsync(Guid id)
        {
            var meetingMinutes = await _context.MeetingMinutes.Where(mm => mm.Id == id && mm.IsDeleted == false)
                                                              .Select(res => new MeetingMinutesResponseModel
                                                              {
                                                                  Id = res.Id,
                                                                  Title = res.Title,
                                                                  MeetingNo = res.MeetingNo,
                                                                  Description = res.Description,
                                                                  MeetingDate = res.MeetingDate
                                                              }).FirstOrDefaultAsync();

            if (meetingMinutes == null)
            {
                message = "Meeting minutes not found";
                return new ApiResponse<MeetingMinutesResponseModel>(message: message, statusCode: (int)HttpStatusCode.NotFound); // 404
            }

            return new ApiResponse<MeetingMinutesResponseModel>
            {
                StatusCode = (int)HttpStatusCode.OK, // 200
                Message = "Meeting minutes retrieved successfully",
                IsSuccess = true,
                Data = meetingMinutes
            };
        }
    }
}
