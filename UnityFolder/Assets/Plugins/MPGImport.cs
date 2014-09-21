using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
 
 
   
        public class MPGImport
        {          
                const string Mpg123Dll = @"libmpg123-0";
       
        #region enums
                public enum mpg123_parms
                {
                        MPG123_VERBOSE,         /**< set verbosity value for enabling messages to stderr, >= 0 makes sense (integer) */
                        MPG123_FLAGS,           /**< set all flags, p.ex val = MPG123_GAPLESS|MPG123_MONO_MIX (integer) */
                        MPG123_ADD_FLAGS,       /**< add some flags (integer) */
                        MPG123_FORCE_RATE,      /**< when value > 0, force output rate to that value (integer) */
                        MPG123_DOWN_SAMPLE,     /**< 0=native rate, 1=half rate, 2=quarter rate (integer) */
                        MPG123_RVA,             /**< one of the RVA choices above (integer) */
                        MPG123_DOWNSPEED,       /**< play a frame N times (integer) */
                        MPG123_UPSPEED,         /**< play every Nth frame (integer) */
                        MPG123_START_FRAME,     /**< start with this frame (skip frames before that, integer) */
                        MPG123_DECODE_FRAMES,   /**< decode only this number of frames (integer) */
                        MPG123_ICY_INTERVAL,    /**< stream contains ICY metadata with this interval (integer) */
                        MPG123_OUTSCALE,        /**< the scale for output samples (amplitude - integer or float according to mpg123 output format, normally integer) */
                        MPG123_TIMEOUT,         /**< timeout for reading from a stream (not supported on win32, integer) */
                        MPG123_REMOVE_FLAGS,    /**< remove some flags (inverse of MPG123_ADD_FLAGS, integer) */
                        MPG123_RESYNC_LIMIT,    /**< Try resync on frame parsing for that many bytes or until end of stream (<0 ... integer). */
                        MPG123_INDEX_SIZE,      /**< Set the frame index size (if supported). Values <0 mean that the index is allowed to grow dynamically in these steps (in positive direction, of course) -- Use this when you really want a full index with every individual frame. */
                        MPG123_PREFRAMES        /**< Decode/ignore that many frames in advance for layer 3. This is needed to fill bit reservoir after seeking, for example (but also at least one frame in advance is needed to have all "normal" data for layer 3). Give a positive integer value, please.*/
                };
 
                public enum mpg123_param_flags
                {
                        MPG123_FORCE_MONO = 0x7,    /**<     0111 Force some mono mode: This is a test bitmask for seeing if any mono forcing is active. */
                        MPG123_MONO_LEFT = 0x1,     /**<     0001 Force playback of left channel only.  */
                        MPG123_MONO_RIGHT = 0x2,    /**<     0010 Force playback of right channel only. */
                        MPG123_MONO_MIX = 0x4,      /**<     0100 Force playback of mixed mono.         */
                        MPG123_FORCE_STEREO = 0x8,  /**<     1000 Force stereo output.                  */
                        MPG123_FORCE_8BIT = 0x10,   /**< 00010000 Force 8bit formats.                   */
                        MPG123_QUIET = 0x20,        /**< 00100000 Suppress any printouts (overrules verbose).                    */
                        MPG123_GAPLESS = 0x40,      /**< 01000000 Enable gapless decoding (default on if libmpg123 has support). */
                        MPG123_NO_RESYNC = 0x80,    /**< 10000000 Disable resync stream after error.                             */
                        MPG123_SEEKBUFFER = 0x100,  /**< 000100000000 Enable small buffer on non-seekable streams to allow some peek-ahead (for better MPEG sync). */
                        MPG123_FUZZY = 0x200,       /**< 001000000000 Enable fuzzy seeks (guessing byte offsets or using approximate seek points from Xing TOC) */
                        MPG123_FORCE_FLOAT = 0x400, /**< 010000000000 Force floating point output (32 or 64 bits depends on mpg123 internal precision). */
                        MPG123_PLAIN_ID3TEXT = 0x800,       /**< 100000000000 Do not translate ID3 text data to UTF-8. ID3 strings will contain the raw text data, with the first byte containing the ID3 encoding code. */
                        MPG123_IGNORE_STREAMLENGTH = 0x1000 /**< 1000000000000 Ignore any stream length information contained in the stream, which can be contained in a 'TLEN' frame of an ID3v2 tag or a Xing tag */
                };
 
                /** choices for MPG123_RVA */
                public enum mpg123_param_rva
                {
                        MPG123_RVA_OFF = 0,     /**< RVA disabled (default).   */
                        MPG123_RVA_MIX = 1,     /**< Use mix/track/radio gain. */
                        MPG123_RVA_ALBUM = 2,   /**< Use album/audiophile gain */
                        MPG123_RVA_MAX = MPG123_RVA_ALBUM /**< The maximum RVA code, may increase in future. */
                };
 
                public enum mpg123_feature_set
                {
                        MPG123_FEATURE_ABI_UTF8OPEN = 0,     /**< mpg123 expects path names to be given in UTF-8 encoding instead of plain native. */
                        MPG123_FEATURE_OUTPUT_8BIT,          /**< 8bit output   */
                        MPG123_FEATURE_OUTPUT_16BIT,         /**< 16bit output  */
                        MPG123_FEATURE_OUTPUT_32BIT,         /**< 32bit output  */
                        MPG123_FEATURE_INDEX,                /**< support for building a frame index for accurate seeking */
                        MPG123_FEATURE_PARSE_ID3V2,          /**< id3v2 parsing */
                        MPG123_FEATURE_DECODE_LAYER1,        /**< mpeg layer-1 decoder enabled */
                        MPG123_FEATURE_DECODE_LAYER2,        /**< mpeg layer-2 decoder enabled */
                        MPG123_FEATURE_DECODE_LAYER3,        /**< mpeg layer-3 decoder enabled */
                        MPG123_FEATURE_DECODE_ACCURATE,      /**< accurate decoder rounding    */
                        MPG123_FEATURE_DECODE_DOWNSAMPLE,    /**< downsample (sample omit)     */
                        MPG123_FEATURE_DECODE_NTOM,          /**< flexible rate decoding       */
                        MPG123_FEATURE_PARSE_ICY,            /**< ICY support                  */
                        MPG123_FEATURE_TIMEOUT_READ          /**< Reader with timeout (network). */
                };
 
                public enum mpg123_errors
                {
                        MPG123_DONE = -12,          /**< Message: Track ended. Stop decoding. */
                        MPG123_NEW_FORMAT = -11,/**< Message: Output format will be different on next call. Note that some libmpg123 versions between 1.4.3 and 1.8.0 insist on you calling mpg123_getformat() after getting this message code. Newer verisons behave like advertised: You have the chance to call mpg123_getformat(), but you can also just continue decoding and get your data. */
                        MPG123_NEED_MORE = -10, /**< Message: For feed reader: "Feed me more!" (call mpg123_feed() or mpg123_decode() with some new input data). */
                        MPG123_ERR = -1,                /**< Generic Error */
                        MPG123_OK = 0,                  /**< Success */
                        MPG123_BAD_OUTFORMAT,   /**< Unable to set up output format! */
                        MPG123_BAD_CHANNEL,             /**< Invalid channel number specified. */
                        MPG123_BAD_RATE,                /**< Invalid sample rate specified.  */
                        MPG123_ERR_16TO8TABLE,  /**< Unable to allocate memory for 16 to 8 converter table! */
                        MPG123_BAD_PARAM,               /**< Bad parameter id! */
                        MPG123_BAD_BUFFER,              /**< Bad buffer given -- invalid pointer or too small size. */
                        MPG123_OUT_OF_MEM,              /**< Out of memory -- some malloc() failed. */
                        MPG123_NOT_INITIALIZED, /**< You didn't initialize the library! */
                        MPG123_BAD_DECODER,             /**< Invalid decoder choice. */
                        MPG123_BAD_HANDLE,              /**< Invalid mpg123 handle. */
                        MPG123_NO_BUFFERS,              /**< Unable to initialize frame buffers (out of memory?). */
                        MPG123_BAD_RVA,                 /**< Invalid RVA mode. */
                        MPG123_NO_GAPLESS,              /**< This build doesn't support gapless decoding. */
                        MPG123_NO_SPACE,                /**< Not enough buffer space. */
                        MPG123_BAD_TYPES,               /**< Incompatible numeric data types. */
                        MPG123_BAD_BAND,                /**< Bad equalizer band. */
                        MPG123_ERR_NULL,                /**< Null pointer given where valid storage address needed. */
                        MPG123_ERR_READER,              /**< Error reading the stream. */
                        MPG123_NO_SEEK_FROM_END,/**< Cannot seek from end (end is not known). */
                        MPG123_BAD_WHENCE,              /**< Invalid 'whence' for seek function.*/
                        MPG123_NO_TIMEOUT,              /**< Build does not support stream timeouts. */
                        MPG123_BAD_FILE,                /**< File access error. */
                        MPG123_NO_SEEK,                 /**< Seek not supported by stream. */
                        MPG123_NO_READER,               /**< No stream opened. */
                        MPG123_BAD_PARS,                /**< Bad parameter handle. */
                        MPG123_BAD_INDEX_PAR,   /**< Bad parameters to mpg123_index() and mpg123_set_index() */
                        MPG123_OUT_OF_SYNC,         /**< Lost track in bytestream and did not try to resync. */
                        MPG123_RESYNC_FAIL,         /**< Resync failed to find valid MPEG data. */
                        MPG123_NO_8BIT,         /**< No 8bit encoding possible. */
                        MPG123_BAD_ALIGN,           /**< Stack aligmnent error */
                        MPG123_NULL_BUFFER,         /**< NULL input buffer with non-zero size... */
                        MPG123_NO_RELSEEK,          /**< Relative seek not possible (screwed up file offset) */
                        MPG123_NULL_POINTER,    /**< You gave a null pointer somewhere where you shouldn't have. */
                        MPG123_BAD_KEY,         /**< Bad key value given. */
                        MPG123_NO_INDEX,            /**< No frame index in this build. */
                        MPG123_INDEX_FAIL,          /**< Something with frame index went wrong. */
                        MPG123_BAD_DECODER_SETUP,       /**< Something prevents a proper decoder setup */
                        MPG123_MISSING_FEATURE, /**< This feature has not been built into libmpg123. */
                        MPG123_BAD_VALUE,       /**< A bad value has been given, somewhere. */
                        MPG123_LSEEK_FAILED,    /**< Low-level seek failed. */
                        MPG123_BAD_CUSTOM_IO,   /**< Custom I/O not prepared. */
                        MPG123_LFS_OVERFLOW     /**< Offset value overflow during translation of large file API calls -- your client program cannot handle that large file. */
                };
 
                public enum mpg123_enc_enum
                {
                        MPG123_ENC_8 = 0x00f,       /**< 0000 0000 1111 Some 8 bit  integer encoding. */
                        MPG123_ENC_16 = 0x040,      /**< 0000 0100 0000 Some 16 bit integer encoding. */
                        MPG123_ENC_32 = 0x100,      /**< 0001 0000 0000 Some 32 bit integer encoding. */
                        MPG123_ENC_SIGNED = 0x080,  /**< 0000 1000 0000 Some signed integer encoding. */
                        MPG123_ENC_FLOAT = 0xe00,   /**< 1110 0000 0000 Some float encoding. */
                        MPG123_ENC_SIGNED_16 = (MPG123_ENC_16 | MPG123_ENC_SIGNED | 0x10),  /**<           1101 0000 signed 16 bit */
                        MPG123_ENC_UNSIGNED_16 = (MPG123_ENC_16 | 0x20),                    /**<           0110 0000 unsigned 16 bit */
                        MPG123_ENC_UNSIGNED_8 = 0x01,                                       /**<           0000 0001 unsigned 8 bit */
                        MPG123_ENC_SIGNED_8 = (MPG123_ENC_SIGNED | 0x02),                   /**<           1000 0010 signed 8 bit */
                        MPG123_ENC_ULAW_8 = 0x04,                                           /**<           0000 0100 ulaw 8 bit */
                        MPG123_ENC_ALAW_8 = 0x08,                                           /**<           0000 1000 alaw 8 bit */
                        MPG123_ENC_SIGNED_32 = MPG123_ENC_32 | MPG123_ENC_SIGNED | 0x1000,  /**< 0001 0001 1000 0000 signed 32 bit */
                        MPG123_ENC_UNSIGNED_32 = MPG123_ENC_32 | 0x2000,                    /**< 0010 0001 0000 0000 unsigned 32 bit */
                        MPG123_ENC_FLOAT_32 = 0x200,                                        /**<      0010 0000 0000 32bit float */
                        MPG123_ENC_FLOAT_64 = 0x400,                                        /**<      0100 0000 0000 64bit float */
                        MPG123_ENC_ANY = (MPG123_ENC_SIGNED_16 | MPG123_ENC_UNSIGNED_16 | MPG123_ENC_UNSIGNED_8
                                                         | MPG123_ENC_SIGNED_8 | MPG123_ENC_ULAW_8 | MPG123_ENC_ALAW_8
                                                         | MPG123_ENC_SIGNED_32 | MPG123_ENC_UNSIGNED_32
                                                         | MPG123_ENC_FLOAT_32 | MPG123_ENC_FLOAT_64)       /**< any encoding */
                };
 
                /** They can be combined into one number (3) to indicate mono and stereo... */
                public enum mpg123_channelcount
                {
                        MPG123_MONO = 1,
                        MPG123_STEREO = 2
                };
 
                public enum mpg123_channels
                {
                        MPG123_LEFT = 0x1,      /**< The Left Channel. */
                        MPG123_RIGHT = 0x2,     /**< The Right Channel. */
                        MPG123_LR = 0x3     /**< Both left and right channel; same as MPG123_LEFT|MPG123_RIGHT */
                };
 
                /** Enumeration of the mode types of Variable Bitrate */
                public enum mpg123_vbr {
                        MPG123_CBR=0,   /**< Constant Bitrate Mode (default) */
                        MPG123_VBR,             /**< Variable Bitrate Mode */
                        MPG123_ABR              /**< Average Bitrate Mode */
                };
 
                /** Enumeration of the MPEG Versions */
                public enum mpg123_version {
                        MPG123_1_0=0,   /**< MPEG Version 1.0 */
                        MPG123_2_0,             /**< MPEG Version 2.0 */
                        MPG123_2_5              /**< MPEG Version 2.5 */
                };
 
                /** Enumeration of the MPEG Audio mode.
                 *  Only the mono mode has 1 channel, the others have 2 channels. */
                public enum mpg123_mode {
                        MPG123_M_STEREO=0,      /**< Standard Stereo. */
                        MPG123_M_JOINT,         /**< Joint Stereo. */
                        MPG123_M_DUAL,          /**< Dual Channel. */
                        MPG123_M_MONO           /**< Single Channel. */
                };
 
                /** Enumeration of the MPEG Audio flag bits */
                public enum mpg123_flags {
                        MPG123_CRC=0x1,                 /**< The bitstream is error protected using 16-bit CRC. */
                        MPG123_COPYRIGHT=0x2,   /**< The bitstream is copyrighted. */
                        MPG123_PRIVATE=0x4,             /**< The private bit has been set. */
                        MPG123_ORIGINAL=0x8     /**< The bitstream is an original, not a copy. */
                };
 
                public enum mpg123_state
                {
                        MPG123_ACCURATE = 1 /**< Query if positons are currently accurate (integer value, 0 if false, 1 if true) */
                };
 
                public enum mpg123_text_encoding
                {
                        mpg123_text_unknown = 0,    /**< Unkown encoding... mpg123_id3_encoding can return that on invalid codes. */
                        mpg123_text_utf8 = 1,       /**< UTF-8 */
                        mpg123_text_latin1 = 2,     /**< ISO-8859-1. Note that sometimes latin1 in ID3 is abused for totally different encodings. */
                        mpg123_text_icy = 3,        /**< ICY metadata encoding, usually CP-1252 but we take it as UTF-8 if it qualifies as such. */
                        mpg123_text_cp1252 = 4,     /**< Really CP-1252 without any guessing. */
                        mpg123_text_utf16 = 5,      /**< Some UTF-16 encoding. The last of a set of leading BOMs (byte order mark) rules.
                                                                                 *   When there is no BOM, big endian ordering is used. Note that UCS-2 qualifies as UTF-8 when
                                                                                 *   you don't mess with the reserved code points. If you want to decode little endian data
                                                                                 *   without BOM you need to prepend 0xff 0xfe yourself. */
                        mpg123_text_utf16bom = 6,   /**< Just an alias for UTF-16, ID3v2 has this as distinct code. */
                        mpg123_text_utf16be = 7,    /**< Another alias for UTF16 from ID3v2. Note, that, because of the mess that is reality,
                                                                                 *   BOMs are used if encountered. There really is not much distinction between the UTF16 types for mpg123
                                                                                 *   One exception: Since this is seen in ID3v2 tags, leading null bytes are skipped for all other UTF16
                                                                                 *   types (we expect a BOM before real data there), not so for utf16be!*/
                        mpg123_text_max = 7         /**< Placeholder for the maximum encoding value. */
                };
 
                /** The encoding byte values from ID3v2. */
                public enum mpg123_id3_enc
                {
                        mpg123_id3_latin1 = 0,      /**< Note: This sometimes can mean anything in practice... */
                        mpg123_id3_utf16bom = 1,    /**< UTF16, UCS-2 ... it's all the same for practical purposes. */
                        mpg123_id3_utf16be = 2,     /**< Big-endian UTF-16, BOM see note for mpg123_text_utf16be. */
                        mpg123_id3_utf8 = 3,        /**< Our lovely overly ASCII-compatible 8 byte encoding for the world. */
                        mpg123_id3_enc_max = 3      /**< Placeholder to check valid range of encoding byte. */
                };
 
 
 
 
 
 
        #endregion
 
        #region Structs
 
                /** Data structure for storing information about a frame of MPEG Audio */
                [StructLayout(LayoutKind.Sequential)]
                public struct mpg123_frameinfo
                {
                        mpg123_version version; /**< The MPEG version (1.0/2.0/2.5). */
                        int layer;                              /**< The MPEG Audio Layer (MP1/MP2/MP3). */
                        Int32 rate;                     /**< The sampling rate in Hz. */
                        mpg123_mode mode;               /**< The audio mode (Mono, Stereo, Joint-stero, Dual Channel). */
                        int mode_ext;                   /**< The mode extension bit flag. */
                        int framesize;                  /**< The size of the frame (in bytes). */
                        mpg123_flags flags;             /**< MPEG Audio flag bits. Just now I realize that it should be declared as int, not enum. It's a bitwise combination of the enum values. */
                        int emphasis;                   /**< The emphasis type. */
                        int bitrate;                    /**< Bitrate of the frame (kbps). */
                        int abr_rate;                   /**< The target average bitrate. */
                        mpg123_vbr vbr;                 /**< The VBR mode. */
                };
 
                [StructLayout(LayoutKind.Sequential)]
                public struct mpg123_string
                {
                        string p; /**< pointer to the string data */
                        int size; /**< raw number of bytes allocated */
                        int fill; /**< number of used bytes (including closing zero byte) */
                }
 
                [StructLayout(LayoutKind.Sequential)]
                public struct mpg123_text
                {
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
                        char[] lang;                /**< Three-letter language code (not terminated). */
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                        char[] id;                  /**< The ID3v2 text field id, like TALB, TPE2, ... (4 characters, no string termination). */
                        mpg123_string description;  /**< Empty for the generic comment... */
                        mpg123_string text;         /**< ... */
                }
 
                public struct mpg123_id3v2
                {
                        byte version;           /**< 3 or 4 for ID3v2.3 or ID3v2.4. */
                        IntPtr title;           /**< Title string (pointer into text_list). */
                        IntPtr artist;          /**< Artist string (pointer into text_list). */
                        IntPtr album;           /**< Album string (pointer into text_list). */
                        IntPtr year;            /**< The year as a string (pointer into text_list). */
                        IntPtr genre;           /**< Genre String (pointer into text_list). The genre string(s) may very well need postprocessing, esp. for ID3v2.3. */
                        IntPtr comment;         /**< Pointer to last encountered comment text with empty description. */
                        IntPtr comment_list;    /**< Array of comments. */
                        int comments;           /**< Number of comments. */
                        IntPtr text;            /**< Array of ID3v2 text fields (including USLT) */
                        int texts;              /**< Numer of text fields. */
                        IntPtr extra;           /**< The array of extra (TXXX) fields. */
                        int extras;             /**< Number of extra text (TXXX) fields. */
                }
 
                public struct mpg123_id3v1
                {
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
                        public char[] tag;         /**< Always the string "TAG", the classic intro. */
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public char[] title;      /**< Title string.  */
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public char[] artist;     /**< Artist string. */
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public char[] album;      /**< Album string. */
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] year;        /**< Year string. */
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public char[] comment;    /**< Comment string. */
            public byte genre;        /**< Genre index. */
                }
 
 
                #endregion
 
        #region Imported methods
 
                [DllImport(Mpg123Dll)]public static extern int mpg123_init();
                [DllImport(Mpg123Dll)]public static extern void mpg123_exit();
                [DllImport(Mpg123Dll, CharSet = CharSet.Ansi)]public static extern IntPtr mpg123_new(string decoder, IntPtr error);
                [DllImport(Mpg123Dll)]public static extern void mpg123_delete(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_param(IntPtr mh, mpg123_parms type, Int32 value, double fvalue);
                [DllImport(Mpg123Dll)]public static extern int mpg123_getparam(IntPtr mh, mpg123_parms type, IntPtr val, IntPtr fval);
                [DllImport(Mpg123Dll)]public static extern int mpg123_feature(mpg123_feature_set key);
                [DllImport(Mpg123Dll, CharSet = CharSet.Ansi)]public static extern string mpg123_plain_strerror(int errcode);
                [DllImport(Mpg123Dll, CharSet = CharSet.Ansi)]public static extern string mpg123_strerror(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_errcode(IntPtr mh);              
                [DllImport(Mpg123Dll, CharSet=CharSet.Ansi)]public static extern IntPtr mpg123_decoders();
                [DllImport(Mpg123Dll, CharSet = CharSet.Ansi)]public static extern IntPtr mpg123_supported_decoders();
                [DllImport(Mpg123Dll)]public static extern int mpg123_decoder(IntPtr mh, string decoder_name);
                [DllImport(Mpg123Dll)]public static extern string mpg123_current_decoder(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern void mpg123_rates(IntPtr list, IntPtr number);
                [DllImport(Mpg123Dll)]public static extern void mpg123_encodings(IntPtr list, IntPtr number);
                [DllImport(Mpg123Dll)]public static extern int mpg123_format_none(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_format_all(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_format(IntPtr mh, int rate, int channels, int encodings);
                [DllImport(Mpg123Dll)]public static extern int mpg123_format_support(IntPtr mh, Int32 rate, int encoding);
                [DllImport(Mpg123Dll)]public static extern int mpg123_getformat(IntPtr mh, out IntPtr rate, out IntPtr channels, out IntPtr encoding);
                [DllImport(Mpg123Dll, CharSet=CharSet.Ansi)]public static extern int mpg123_open(IntPtr mh, string path);
                [DllImport(Mpg123Dll)]public static extern int mpg123_open_fd(IntPtr mh, int fd);
                [DllImport(Mpg123Dll)]public static extern int mpg123_open_handle(IntPtr mh, IntPtr iohandle);
                [DllImport(Mpg123Dll)]public static extern int mpg123_open_feed(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_close(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_read(IntPtr mh, byte[] outmemory, int outmemsize, out IntPtr done);
                [DllImport(Mpg123Dll)]public static extern int mpg123_feed(IntPtr mh, IntPtr input, int size);
                [DllImport(Mpg123Dll)]public static extern int mpg123_decode(IntPtr mh, IntPtr inmemory, int inmemsize, IntPtr outmemory, int outmemsize, IntPtr done);
                [DllImport(Mpg123Dll)]public static extern int mpg123_decode_frame(IntPtr mh, IntPtr num, IntPtr audio, IntPtr bytes);
                [DllImport(Mpg123Dll)]public static extern int mpg123_framebyframe_decode(IntPtr mh, IntPtr num, IntPtr audio, IntPtr bytes);
                [DllImport(Mpg123Dll)]public static extern int mpg123_framebyframe_next(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_tell(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_tellframe(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_tell_stream(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_seek(IntPtr mh, int sampleoff, int whence);
                [DllImport(Mpg123Dll)]public static extern int mpg123_feedseek(IntPtr mh, int sampleoff, int whence, IntPtr input_offset);
                [DllImport(Mpg123Dll)]public static extern int mpg123_seek_frame(IntPtr mh, int frameoff, int whence);
                [DllImport(Mpg123Dll)]public static extern int mpg123_timeframe(IntPtr mh, double sec);
                [DllImport(Mpg123Dll)]public static extern int mpg123_index(IntPtr mh, IntPtr offsets, IntPtr step, IntPtr fill);
                [DllImport(Mpg123Dll)]public static extern int mpg123_set_index(IntPtr mh, IntPtr offsets, int step, int fill);
                [DllImport(Mpg123Dll)]public static extern int mpg123_position( IntPtr mh, int frame_offset, int buffered_bytes, IntPtr current_frame, IntPtr frames_left, IntPtr current_seconds, IntPtr seconds_left);
                [DllImport(Mpg123Dll)]public static extern int mpg123_eq(IntPtr mh, mpg123_channels channel, int band, double val);
                [DllImport(Mpg123Dll)]public static extern double mpg123_geteq(IntPtr mh, mpg123_channels channel, int band);
                [DllImport(Mpg123Dll)]public static extern int mpg123_reset_eq(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_volume(IntPtr mh, double vol);
                [DllImport(Mpg123Dll)]public static extern int mpg123_volume_change(IntPtr mh, double change);
                [DllImport(Mpg123Dll)]public static extern int mpg123_getvolume(IntPtr mh, IntPtr _base, IntPtr really, IntPtr rva_db);
                [DllImport(Mpg123Dll)]public static extern int mpg123_info(IntPtr mh, IntPtr mi);
                [DllImport(Mpg123Dll)]public static extern int mpg123_safe_buffer();
                [DllImport(Mpg123Dll)]public static extern int mpg123_scan(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_length(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_set_filesize(IntPtr mh, int size);
                [DllImport(Mpg123Dll)]public static extern double mpg123_tpf(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern Int32 mpg123_clip(IntPtr mh);
                [DllImport(Mpg123Dll)]public static extern int mpg123_getstate(IntPtr mh, mpg123_state key, IntPtr val, IntPtr fval);
                [DllImport(Mpg123Dll)]public static extern void mpg123_init_string(IntPtr sb);
                [DllImport(Mpg123Dll)]public static extern void mpg123_free_string(IntPtr sb);
                [DllImport(Mpg123Dll)]public static extern int  mpg123_resize_string(IntPtr sb, int news);
                [DllImport(Mpg123Dll)]public static extern int  mpg123_grow_string(IntPtr sb, int news);
                [DllImport(Mpg123Dll)]public static extern int  mpg123_copy_string(IntPtr from, IntPtr to);
                [DllImport(Mpg123Dll)]public static extern int  mpg123_add_string(IntPtr sb, string stuff);
                [DllImport(Mpg123Dll)]public static extern int  mpg123_add_substring(IntPtr sb, string stuff, int from, int count);
                [DllImport(Mpg123Dll)]public static extern int  mpg123_set_string(IntPtr sb, string stuff);
                [DllImport(Mpg123Dll)]public static extern int  mpg123_set_substring(IntPtr sb, string stuff, int from, int count);
                [DllImport(Mpg123Dll)]public static extern mpg123_text_encoding mpg123_enc_from_id3(byte id3_enc_byte);
                [DllImport(Mpg123Dll)]public static extern int mpg123_store_utf8(IntPtr sb, mpg123_text_encoding enc, string source, int source_size);
                [DllImport(Mpg123Dll)]public static extern int mpg123_meta_check(IntPtr mh); /* On error (no valid handle) just 0 is returned. */
                [DllImport(Mpg123Dll)]public static extern int mpg123_id3(IntPtr mh, out IntPtr v1, out IntPtr v2);
                [DllImport(Mpg123Dll)]public static extern int mpg123_icy(IntPtr mh, IntPtr icy_meta); /* same for ICY meta string */
                [DllImport(Mpg123Dll)]public static extern string mpg123_icy2utf8(string icy_text);
                [DllImport(Mpg123Dll)]public static extern IntPtr mpg123_parnew(IntPtr mp, string decoder, IntPtr error);
                [DllImport(Mpg123Dll)]public static extern IntPtr mpg123_new_pars(IntPtr error);
                [DllImport(Mpg123Dll)]public static extern void mpg123_delete_pars(IntPtr mp);
                [DllImport(Mpg123Dll)]public static extern int mpg123_fmt_none(IntPtr mp);
                [DllImport(Mpg123Dll)]public static extern int mpg123_fmt_all(IntPtr mp);
                [DllImport(Mpg123Dll)]public static extern int mpg123_fmt(IntPtr mh, Int32 rate, int channels, int encodings); /* 0 is good, -1 is error */
                [DllImport(Mpg123Dll)]public static extern int mpg123_fmt_support(IntPtr mh,   Int32 rate, int encoding);
                [DllImport(Mpg123Dll)]public static extern int mpg123_par(IntPtr mp, mpg123_parms type, Int32 value, double fvalue);
                [DllImport(Mpg123Dll)]public static extern int mpg123_getpar(IntPtr mp, mpg123_parms type, IntPtr val, IntPtr fval);
                [DllImport(Mpg123Dll)]public static extern int mpg123_replace_buffer(IntPtr mh, string data, int size);
                [DllImport(Mpg123Dll)]public static extern int mpg123_outblock(IntPtr mh);
 
        #endregion
 
        }