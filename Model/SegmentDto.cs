namespace WebApi_KR.Model
{
    /// <summary>
    /// Represents segment data transfer object that carries message's queue ID and segment bit data.
    /// </summary>
    public class SegmentDto
    {
        /// <summary>
        /// The bit code representation of the segment data. Up to 130 bits.
        /// </summary>
        /// <example>11111111111000000000</example>
        public string Segment { get; set; } = null!;

        /// <summary>
        /// Meta. The ID's of the segment's message owner queue.
        /// </summary>
        /// <example>0</example>
        public string Message_id { get; set; } = null!;

        /// <summary>
        /// Meta. Current segment number.
        /// </summary>
        /// <example>0</example>
        public int Segment_number { get; set; }

        /// <summary>
        /// Meta. Message send time.
        /// </summary>
        /// <example>1713607200</example>
        public string Send_time { get; set; } = null!;

        /// <summary>
        /// Meta. Total segments count.
        /// </summary>
        /// <example>20</example>
        public int Total_segments { get; set; }

        /// <summary>
        /// Meta. Sender username.
        /// </summary>
        /// <example>Admin</example>
        public string Username { get; set; } = null!;
    }
}