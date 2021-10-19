
namespace AmhMailServer
{


    public delegate void foo_t(string args);


    public enum Pop3Command
    {

        STLS,
        USER,
        PASS,
        AUTH,
        STAT,
        LIST,
        UIDL,
        TOP,
        RETR,
        DELE,
        NOOP,
        RSET,
        CAPA,
        QUIT
    }


    public class InternalMailMessage
    {
        static byte[] buffer = System.IO.File.ReadAllBytes(@"C:\Users\User\Downloads\enron_mail_20150507\maildir\harris-s\inbox\1");

        public bool IsMarkedForDeletion;
        public byte[] Buffer;
        public int SequenceNumber;
        public string UID;
        


        public int Size
        {
            get
            {
                return Buffer.Length;
            }
        }

        public InternalMailMessage()
        {
            this.Buffer = buffer;
        }


        public void SetIsMarkedForDeletion(bool b)
        {
            this.IsMarkedForDeletion = b;
        }

    }


    public class MailMessageStat
    {

        static byte[] buffer = System.IO.File.ReadAllBytes(@"C:\Users\User\Downloads\enron_mail_20150507\maildir\harris-s\inbox\1");

        // public int Size;
        public bool IsMarkedForDeletion;

        public int Size
        {
            get
            {
                return buffer.Length;
            }
        }



        //public byte[] Buffer;
        //public System.IO.Stream Stream;
    }


    public class Pop3TcpSession
        : NetCoreServer.TcpSession
    {

        string[] commands;
        System.Collections.Generic.Dictionary<string, foo_t> commandHandlers;


        protected bool m_SessionRejected;
        protected bool IsAuthenticated;
        protected string m_UserName;


        protected string m_currentCommand;


        public void foo(string args)
        {
            System.Console.WriteLine(this.m_currentCommand);
            System.Console.WriteLine(args);
            
            this.Write("-ERR no such message.");
            throw new System.NotImplementedException(this.m_currentCommand);
        }

        private void QUIT(string cmdText)
        {
            /* RFC 1939 6. QUIT
			   NOTE:
                When the client issues the QUIT command from the TRANSACTION state,
				the POP3 session enters the UPDATE state.  (Note that if the client
				issues the QUIT command from the AUTHORIZATION state, the POP3
				session terminates but does NOT enter the UPDATE state.)

				If a session terminates for some reason other than a client-issued
				QUIT command, the POP3 session does NOT enter the UPDATE state and
				MUST not remove any messages from the maildrop.
             
				The POP3 server removes all messages marked as deleted
				from the maildrop and replies as to the status of this
				operation.  If there is an error, such as a resource
				shortage, encountered while removing messages, the
				maildrop may result in having some or none of the messages
				marked as deleted be removed.  In no case may the server
				remove any messages not marked as deleted.

				Whether the removal was successful or not, the server
				then releases any exclusive-access lock on the maildrop
				and closes the TCP connection.
			*/

            try
            {
                //if (this.IsAuthenticated)
                //{
                //    // Delete messages marked for deletion.
                //    foreach (POP3_ServerMessage msg in m_pMessages)
                //    {
                //        if (msg.IsMarkedForDeletion)
                //        {
                //            OnDeleteMessage(msg);
                //        }
                //    }
                //}

                string localHostName = System.Net.Dns.GetHostName();

                // cached lh name
                // WriteLine("+OK <" + Net_Utils.GetLocalHostName(this.LocalHostName) + "> Service closing transmission channel.");
                this.Write("+OK <" + localHostName + "> Service closing transmission channel.");
            }
            catch(System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            this.Disconnect();
            this.Dispose();
        }




        private void CAPA(string cmdText)
        {
            /* RFC 2449 5.  The CAPA Command
			
				The POP3 CAPA command returns a list of capabilities supported by the
				POP3 server.  It is available in both the AUTHORIZATION and
				TRANSACTION states.

				A capability description MUST document in which states the capability
				is announced, and in which states the commands are valid.

				Capabilities available in the AUTHORIZATION state MUST be announced
				in both states.

				If a capability is announced in both states, but the argument might
				differ after authentication, this possibility MUST be stated in the
				capability description.

				(These requirements allow a client to issue only one CAPA command if
				it does not use any TRANSACTION-only capabilities, or any
				capabilities whose values may differ after authentication.)

				If the authentication step negotiates an integrity protection layer,
				the client SHOULD reissue the CAPA command after authenticating, to
				check for active down-negotiation attacks.

				Each capability may enable additional protocol commands, additional
				parameters and responses for existing commands, or describe an aspect
				of server behavior.  These details are specified in the description
				of the capability.
				
				Section 3 describes the CAPA response using [ABNF].  When a
				capability response describes an optional command, the <capa-tag>
				SHOULD be identical to the command keyword.  CAPA response tags are
				case-insensitive.

				CAPA

				Arguments:
					none

				Restrictions:
					none

				Discussion:
					An -ERR response indicates the capability command is not
					implemented and the client will have to probe for
					capabilities as before.

					An +OK response is followed by a list of capabilities, one
					per line.  Each capability name MAY be followed by a single
					space and a space-separated list of parameters.  Each
					capability line is limited to 512 octets (including the
					CRLF).  The capability list is terminated by a line
					containing a termination octet (".") and a CRLF pair.

				Possible Responses:
					+OK -ERR

					Examples:
						C: CAPA
						S: +OK Capability list follows
						S: TOP
						S: USER
						S: SASL CRAM-MD5 KERBEROS_V4
						S: RESP-CODES
						S: LOGIN-DELAY 900
						S: PIPELINING
						S: EXPIRE 60
						S: UIDL
						S: IMPLEMENTATION Shlemazle-Plotz-v302
						S: .
			*/

            //if (m_SessionRejected)
            //{
            //    WriteLine("-ERR Bad sequence of commands: Session rejected.");

            //    return;
            //}

            System.Text.StringBuilder capaResponse = new System.Text.StringBuilder();
            capaResponse.Append("+OK Capability list follows\r\n");
            capaResponse.Append("PIPELINING\r\n");
            capaResponse.Append("UIDL\r\n");
            capaResponse.Append("TOP\r\n");

            System.Text.StringBuilder sasl = new System.Text.StringBuilder();
            //foreach (AUTH_SASL_ServerMechanism authMechanism in this.Authentications.Values)
            //{
            //    if (!authMechanism.RequireSSL || (authMechanism.RequireSSL && this.IsSecureConnection))
            //    {
            //        sasl.Append(authMechanism.Name + " ");
            //    }
            //}
            if (sasl.Length > 0)
            {
                capaResponse.Append("SASL " + sasl.ToString().Trim() + "\r\n");
            }

            //if (!this.IsSecureConnection && this.Certificate != null)
            //{
            //    capaResponse.Append("STLS\r\n");
            //}

            capaResponse.Append(".");

            this.Write(capaResponse.ToString());
        }

        private void NOOP(string cmdText)
        {
            /* RFC 1939 5. NOOP
			    NOTE:
				    The POP3 server does nothing, it merely replies with a
				    positive response.
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            this.Write("+OK");
        }

        private void USER(string cmdText)
        {
            /* RFC 1939 7. USER
			    Arguments:
				    a string identifying a mailbox (required), which is of
				    significance ONLY to the server
				
			    NOTE:
				    If the POP3 server responds with a positive
				    status indicator ("+OK"), then the client may issue
				    either the PASS command to complete the authentication,
				    or the QUIT command to terminate the POP3 session.			 
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (this.IsAuthenticated)
            {
                this.Write("-ERR Re-authentication error.");

                return;
            }
            if (m_UserName != null)
            {
                this.Write("-ERR User name already specified.");

                return;
            }

            m_UserName = cmdText;


            this.Write("+OK User name OK.");
        }

        
        private void PASS(string cmdText)
        {
            this.IsAuthenticated = true;

            this.Write("+OK Authenticated successfully.");
            // this.Write("-ERR Authentication failed.");
        }


        public System.Collections.Generic.List<MailMessageStat> GetMessageStats()
        {
            System.Collections.Generic.List<MailMessageStat> ls = new System.Collections.Generic.List<MailMessageStat>();

            for (int i = 0; i < 10; ++i)
            {
                ls.Add(new MailMessageStat() );
            }

            return ls;
        }


        public System.Collections.Generic.List<InternalMailMessage> GetMessages()
        {
            System.Collections.Generic.List<InternalMailMessage> ls = new System.Collections.Generic.List<InternalMailMessage>();

            for (int i = 0; i < 10; ++i)
            {
                ls.Add(new InternalMailMessage() );
            }

            return ls;
        }


        public bool TryGetValueAt(int i, out InternalMailMessage msg)
        {
            try {
                System.Collections.Generic.List<InternalMailMessage> ls = GetMessages();
                msg = ls[i];
                return true;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            msg = null;
            return false;
        }


        private void STAT(string cmdText)
        {
            /* RFC 1939 5. STAT
			NOTE:
				The positive response consists of "+OK" followed by a single
				space, the number of messages in the maildrop, a single
				space, and the size of the maildrop in octets.
				
				Note that messages marked as deleted are not counted in
				either total.
			 
			Example:
				C: STAT
				S: +OK 2 320
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            // Calculate count and total size in bytes, exclude marked for deletion messages.
            int count = 0;
            int size = 0;


            foreach (MailMessageStat msg in this.GetMessageStats())
            {
                if (!msg.IsMarkedForDeletion)
                {
                    count++;
                    size += msg.Size;
                }
            }

            this.Write("+OK " + count + " " + size);
        }

        /// <summary>
        /// Checks if specified string is integer(int/long).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns true if specified string is integer.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        private static bool IsInteger(string value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value");
            }

            long l = 0;

            return long.TryParse(value, out l);
        }


        private void RETR(string cmdText)
        {
            /* RFC 1939 5. RETR
			    Arguments:
				    a message-number (required) which may NOT refer to a
				    message marked as deleted
			 
			    NOTE:
				    If the POP3 server issues a positive response, then the
				    response given is multi-line.  After the initial +OK, the
				    POP3 server sends the message corresponding to the given
				    message-number, being careful to byte-stuff the termination
				    character (as with all multi-line responses).
				
			    Example:
				    C: RETR 1
				    S: +OK 120 octets
				    S: <the POP3 server sends the entire message here>
				    S: .
			
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            string[] args = cmdText.Split(' ');

            if (args.Length != 1 || !IsInteger(args[0]))
            {
                this.Write("-ERR Error in arguments.");

                return;
            }


            InternalMailMessage msg = null;
            if (TryGetValueAt(System.Convert.ToInt32(args[0]) - 1, out msg))
            {
                // Block messages marked for deletion.
                if (msg.IsMarkedForDeletion)
                {
                    this.Write("-ERR Invalid operation: Message marked for deletion.");

                    return;
                }

                // POP3_e_GetMessageStream e = OnGetMessageStream(msg);

                // User didn't provide us message stream, assume that message deleted(for example by IMAP during this POP3 session).
                // if (e.MessageStream == null)
                if(msg == null)
                {
                    this.Write("-ERR no such message.");
                }
                else
                {
                    try
                    {
                        // this.Write("+OK Start sending message.");

                        // long countWritten = this.TcpStream.WritePeriodTerminated(e.MessageStream);

                        this.Write("+OK " + msg.Buffer.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        SendAsync(msg.Buffer);
                        this.Write("\r\n.");


                        // Log.
                        //if (this.Server.Logger != null)
                        //{
                        //    this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, countWritten, "Wrote message(" + countWritten + " bytes).", this.LocalEndPoint, this.RemoteEndPoint);
                        //}
                    }
                    finally
                    {
                        // Close message stream if CloseStream = true.
                        //if (e.CloseMessageStream)
                        //{
                        //    e.MessageStream.Dispose();
                        //}
                    }
                }
            }
            else
            {
                this.Write("-ERR no such message.");
            }
        }



        private void DELE(string cmdText)
        {
            /* RFC 1939 5. DELE
			    Arguments:
				    a message-number (required) which may NOT refer to a
				    message marked as deleted
			 
			    NOTE:
				    The POP3 server marks the message as deleted.  Any future
				    reference to the message-number associated with the message
				    in a POP3 command generates an error.  The POP3 server does
				    not actually delete the message until the POP3 session
				    enters the UPDATE state.
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            string[] args = cmdText.Split(' ');

            if (args.Length != 1 || !IsInteger(args[0]))
            {
                this.Write("-ERR Error in arguments.");

                return;
            }

            InternalMailMessage msg;
            if (TryGetValueAt(System.Convert.ToInt32(args[0]) - 1, out msg))
            {
                if (!msg.IsMarkedForDeletion)
                {
                    msg.SetIsMarkedForDeletion(true);

                    this.Write("+OK Message marked for deletion.");
                }
                else
                {
                    this.Write("-ERR Message already marked for deletion.");
                }
            }
            else
            {
                this.Write("-ERR no such message.");
            }
        }


        private void RSET(string cmdText)
        {
            /* RFC 1939 5. RSET
			Discussion:
				If any messages have been marked as deleted by the POP3
				server, they are unmarked.  The POP3 server then replies
				with a positive response.
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            // Unmark messages marked for deletion.
            foreach (InternalMailMessage msg in this.GetMessages())
            {
                msg.SetIsMarkedForDeletion(false);
            }


            this.Write("+OK");

            // OnReset();
        }

        class InternalMailMessageTop
        {

            public InternalMailMessageTop(InternalMailMessage message, int lines)
            {
                
                if (message == null)
                {
                    throw new System.ArgumentNullException("message");
                }
                if (lines < 0)
                {
                    throw new System.ArgumentException("Argument 'lines' value must be >= 0.", "lines");
                }

                // m_pMessage = message;
                // m_LineCount = lines;

                throw new System.NotImplementedException("InternalMailMessageTop");
            }

            public byte[] Data;
        }


        /// <summary>
        /// Raises <b>GetTopOfMessage</b> event.
        /// </summary>
        /// <param name="message">Message which top data to get.</param>
        /// <param name="lines">Number of message-body lines to get.</param>
        /// <returns>Returns event args.</returns>
        private InternalMailMessageTop OnGetTopOfMessage(InternalMailMessage message, int lines)
        {
            InternalMailMessageTop eArgs = new InternalMailMessageTop(message, lines);

            //if (this.GetTopOfMessage != null)
            //{
            //    this.GetTopOfMessage(this, eArgs);
            //}

            return eArgs;
        }

        private void TOP(string cmdText)
        {
            /* RFC 1939 7. TOP
			    Arguments:
				    a message-number (required) which may NOT refer to to a
				    message marked as deleted, and a non-negative number
				    of lines (required)
		
			    NOTE:
				    If the POP3 server issues a positive response, then the
				    response given is multi-line.  After the initial +OK, the
				    POP3 server sends the headers of the message, the blank
				    line separating the headers from the body, and then the
				    number of lines of the indicated message's body, being
				    careful to byte-stuff the termination character (as with
				    all multi-line responses).
			
			    Examples:
				    C: TOP 1 10
				    S: +OK
				    S: <the POP3 server sends the headers of the
					    message, a blank line, and the first 10 lines
					    of the body of the message>
				    S: .
                    ...
				    C: TOP 100 3
				    S: -ERR no such message
			 
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            string[] args = cmdText.Split(' ');

            if (args.Length != 2 || !IsInteger(args[0]) || !IsInteger(args[1]))
            {
                this.Write("-ERR Error in arguments.");

                return;
            }

            InternalMailMessage msg = null;
            if (TryGetValueAt(System.Convert.ToInt32(args[0]) - 1, out msg))
            {
                // Block messages marked for deletion.
                if (msg.IsMarkedForDeletion)
                {
                    this.Write("-ERR Invalid operation: Message marked for deletion.");

                    return;
                }

                InternalMailMessageTop e = OnGetTopOfMessage(msg, System.Convert.ToInt32(args[1]));
                

                // User didn't provide us message stream, assume that message deleted(for example by IMAP during this POP3 session).
                if (e.Data == null)
                {
                    this.Write("-ERR no such message.");
                }
                else
                {
                    // this.Write("+OK Start sending top of message.");
                    // long countWritten = this.TcpStream.WritePeriodTerminated(new MemoryStream(e.Data));

                    this.Write("+OK " + e.Data.Length.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    SendAsync(e.Data);
                    this.Write("\r\n.");

                    // Log.
                    //if (this.Server.Logger != null)
                    //{
                    //    this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, countWritten, "Wrote top of message(" + countWritten + " bytes).", this.LocalEndPoint, this.RemoteEndPoint);
                    //}
                }
            }
            else
            {
                this.Write("-ERR no such message.");
            }
        }


        private void LIST(string cmdText)
        {
            /* RFC 1939 5. LIST
			Arguments:
				a message-number (optional), which, if present, may NOT
				refer to a message marked as deleted
			 
			NOTE:
				If an argument was given and the POP3 server issues a
				positive response with a line containing information for
				that message.

				If no argument was given and the POP3 server issues a
				positive response, then the response given is multi-line.
				
				Note that messages marked as deleted are not listed.
			
			Examples:
				C: LIST
				S: +OK 2 messages (320 octets)
				S: 1 120				
				S: 2 200
				S: .
				...
				C: LIST 2
				S: +OK 2 200
				...
				C: LIST 3
				S: -ERR no such message, only 2 messages in maildrop
			 
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            string[] args = cmdText.Split(' ');

            // List whole mailbox.
            if (string.IsNullOrEmpty(cmdText))
            {
                // Calculate count and total size in bytes, exclude marked for deletion messages.
                int count = 0;
                int size = 0;
                foreach (InternalMailMessage msg in GetMessages())
                {
                    if (!msg.IsMarkedForDeletion)
                    {
                        count++;
                        size += msg.Size;
                    }
                }

                System.Text.StringBuilder response = new System.Text.StringBuilder();
                response.Append("+OK " + count + " messages (" + size + " bytes).\r\n");
                foreach (InternalMailMessage msg in GetMessages())
                {
                    response.Append(msg.SequenceNumber + " " + msg.Size + "\r\n");
                }
                response.Append(".");

                this.Write(response.ToString());
            }
            // Single message info listing.
            else
            {
                if (args.Length > 1 || !IsInteger(args[0]))
                {
                    this.Write("-ERR Error in arguments.");

                    return;
                }

                InternalMailMessage msg = null;
                if (TryGetValueAt(System.Convert.ToInt32(args[0]) - 1, out msg))
                {
                    // Block messages marked for deletion.
                    if (msg.IsMarkedForDeletion)
                    {
                        this.Write("-ERR Invalid operation: Message marked for deletion.");

                        return;
                    }

                    this.Write("+OK " + msg.SequenceNumber + " " + msg.Size);
                }
                else
                {
                    this.Write("-ERR no such message or message marked for deletion.");
                }
            }
        }

        private void UIDL(string cmdText)
        {
            /* RFC 1939 UIDL [msg]
			Arguments:
			    a message-number (optional), which, if present, may NOT
				refer to a message marked as deleted
				
			NOTE:
				If an argument was given and the POP3 server issues a positive
				response with a line containing information for that message.

				If no argument was given and the POP3 server issues a positive
				response, then the response given is multi-line.  After the
				initial +OK, for each message in the maildrop, the POP3 server
				responds with a line containing information for that message.	
				
			Examples:
				C: UIDL
				S: +OK
				S: 1 whqtswO00WBw418f9t5JxYwZ
				S: 2 QhdPYR:00WBw1Ph7x7
				S: .
				...
				C: UIDL 2
				S: +OK 2 QhdPYR:00WBw1Ph7x7
				...
				C: UIDL 3
				S: -ERR no such message
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (!this.IsAuthenticated)
            {
                this.Write("-ERR Authentication required.");

                return;
            }

            string[] args = cmdText.Split(' ');

            // List whole mailbox.
            if (string.IsNullOrEmpty(cmdText))
            {
                // Calculate count and total size in bytes, exclude marked for deletion messages.
                int count = 0;
                int size = 0;
                foreach (InternalMailMessage msg in GetMessages())
                {
                    if (!msg.IsMarkedForDeletion)
                    {
                        count++;
                        size += msg.Size;
                    }
                }

                System.Text.StringBuilder response = new System.Text.StringBuilder();
                response.Append("+OK " + count + " messages (" + size + " bytes).\r\n");
                foreach (InternalMailMessage msg in GetMessages())
                {
                    response.Append(msg.SequenceNumber + " " + msg.UID + "\r\n");
                }
                response.Append(".");

                this.Write(response.ToString());
            }
            // Single message info listing.
            else
            {
                if (args.Length > 1)
                {
                    this.Write("-ERR Error in arguments.");

                    return;
                }

                InternalMailMessage msg = null;
                if (TryGetValueAt(System.Convert.ToInt32(args[0]) - 1, out msg))
                {
                    // Block messages marked for deletion.
                    if (msg.IsMarkedForDeletion)
                    {
                        this.Write("-ERR Invalid operation: Message marked for deletion.");

                        return;
                    }

                    this.Write("+OK " + msg.SequenceNumber + " " + msg.UID);
                }
                else
                {
                    this.Write("-ERR no such message or message marked for deletion.");
                }
            }
        }


        private void AUTH(string cmdText)
        {
            /* RFC 1734
				
				AUTH mechanism

					Arguments:
						a string identifying an IMAP4 authentication mechanism,
						such as defined by [IMAP4-AUTH].  Any use of the string
						"imap" used in a server authentication identity in the
						definition of an authentication mechanism is replaced with
						the string "pop".
						
					Possible Responses:
						+OK maildrop locked and ready
						-ERR authentication exchange failed

					Restrictions:
						may only be given in the AUTHORIZATION state

					Discussion:
						The AUTH command indicates an authentication mechanism to
						the server.  If the server supports the requested
						authentication mechanism, it performs an authentication
						protocol exchange to authenticate and identify the user.
						Optionally, it also negotiates a protection mechanism for
						subsequent protocol interactions.  If the requested
						authentication mechanism is not supported, the server						
						should reject the AUTH command by sending a negative
						response.

						The authentication protocol exchange consists of a series
						of server challenges and client answers that are specific
						to the authentication mechanism.  A server challenge,
						otherwise known as a ready response, is a line consisting
						of a "+" character followed by a single space and a BASE64
						encoded string.  The client answer consists of a line
						containing a BASE64 encoded string.  If the client wishes
						to cancel an authentication exchange, it should issue a
						line with a single "*".  If the server receives such an
						answer, it must reject the AUTH command by sending a
						negative response.

						A protection mechanism provides integrity and privacy
						protection to the protocol session.  If a protection
						mechanism is negotiated, it is applied to all subsequent
						data sent over the connection.  The protection mechanism
						takes effect immediately following the CRLF that concludes
						the authentication exchange for the client, and the CRLF of
						the positive response for the server.  Once the protection
						mechanism is in effect, the stream of command and response
						octets is processed into buffers of ciphertext.  Each
						buffer is transferred over the connection as a stream of
						octets prepended with a four octet field in network byte
						order that represents the length of the following data.
						The maximum ciphertext buffer length is defined by the
						protection mechanism.

						The server is not required to support any particular
						authentication mechanism, nor are authentication mechanisms
						required to support any protection mechanisms.  If an AUTH
						command fails with a negative response, the session remains
						in the AUTHORIZATION state and client may try another
						authentication mechanism by issuing another AUTH command,
						or may attempt to authenticate by using the USER/PASS or
						APOP commands.  In other words, the client may request
						authentication types in decreasing order of preference,
						with the USER/PASS or APOP command as a last resort.

						Should the client successfully complete the authentication
						exchange, the POP3 server issues a positive response and
						the POP3 session enters the TRANSACTION state.
						
				Examples:
							S: +OK POP3 server ready
							C: AUTH KERBEROS_V4
							S: + AmFYig==
							C: BAcAQU5EUkVXLkNNVS5FRFUAOCAsho84kLN3/IJmrMG+25a4DT
								+nZImJjnTNHJUtxAA+o0KPKfHEcAFs9a3CL5Oebe/ydHJUwYFd
								WwuQ1MWiy6IesKvjL5rL9WjXUb9MwT9bpObYLGOKi1Qh
							S: + or//EoAADZI=
							C: DiAF5A4gA+oOIALuBkAAmw==
							S: +OK Kerberos V4 authentication successful
								...
							C: AUTH FOOBAR
							S: -ERR Unrecognized authentication type
			 
			*/

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (this.IsAuthenticated)
            {
                this.Write("-ERR Re-authentication error.");

                return;
            }


            this.Write("-ERR Re-authentication error."); // Remove me once fixed;

#if false

            string mechanism = cmdText;

            /* MS specific or someone knows where in RFC let me know about this.
                Empty AUTH commands causes authentication mechanisms listing. 
             
                C: AUTH
                S: PLAIN
                S: .
                
                http://msdn.microsoft.com/en-us/library/cc239199.aspx
            */
            if (string.IsNullOrEmpty(mechanism))
            {
                System.Text.StringBuilder resp = new System.Text.StringBuilder();
                throw new System.NotImplementedException();
                resp.Append("+OK\r\n");

                foreach (AUTH_SASL_ServerMechanism m in m_pAuthentications.Values)
                {
                    resp.Append(m.Name + "\r\n");
                }
                resp.Append(".\r\n");

                this.Write(resp.ToString());

                return;
            }



            if (!this.Authentications.ContainsKey(mechanism))
            {
                this.Write("-ERR Not supported authentication mechanism.");
                return;
            }

            byte[] clientResponse = new byte[0];
            AUTH_SASL_ServerMechanism auth = this.Authentications[mechanism];
            auth.Reset();
            while (true)
            {
                byte[] serverResponse = auth.Continue(clientResponse);
                // Authentication completed.
                if (auth.IsCompleted)
                {
                    if (auth.IsAuthenticated)
                    {
                        throw new System.NotImplementedException("GenericIdentity");
                        //m_pUser = new GenericIdentity(auth.UserName, "SASL-" + auth.Name);

                        // Get mailbox messages.
                        //POP3_e_GetMessagesInfo eMessages = OnGetMessagesInfo();
                        //int seqNo = 1;
                        //foreach (POP3_ServerMessage message in eMessages.Messages)
                        //{
                        //    message.SequenceNumber = seqNo++;
                        //    m_pMessages.Add(message.UID, message);
                        //}

                        this.Write("+OK Authentication succeeded.");
                    }
                    else
                    {
                        this.Write("-ERR Authentication credentials invalid.");
                    }
                    break;
                }
                // Authentication continues.
                else
                {
                    // Send server challenge.
                    if (serverResponse.Length == 0)
                    {
                        this.Write("+ ");
                    }
                    else
                    {
                        this.Write("+ " + System.Convert.ToBase64String(serverResponse));
                    }

                    // Read client response. 
                    SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
                    this.TcpStream.ReadLine(readLineOP, false);
                    if (readLineOP.Error != null)
                    {
                        throw readLineOP.Error;
                    }
                    // Log
                    //if (this.Server.Logger != null)
                    //{
                    //    this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, readLineOP.BytesInBuffer, "base64 auth-data", this.LocalEndPoint, this.RemoteEndPoint);
                    //}

                    // Client canceled authentication.
                    if (readLineOP.LineUtf8 == "*")
                    {
                        this.Write("-ERR Authentication canceled.");
                        return;
                    }
                    // We have base64 client response, decode it.
                    else
                    {
                        byte[] clientResponse = null;

                        try
                        {
                            clientResponse = System.Convert.FromBase64String(readLineOP.LineUtf8);
                        }
                        catch
                        {
                            this.Write("-ERR Invalid client response '" + clientResponse + "'.");
                            return;
                        }
                    }
                }
            }

#endif
        }

        protected bool IsSecureConnection;
        protected System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate;

        protected void SwitchToSecure()
        {
            throw new System.NotImplementedException("SwitchToSecure");
        }


        private void STLS(string cmdText)
        {
            /* RFC 2595 4. POP3 STARTTLS extension.
                 Arguments: none

                 Restrictions:
                     Only permitted in AUTHORIZATION state.

                 Discussion:
                     A TLS negotiation begins immediately after the CRLF at the
                     end of the +OK response from the server.  A -ERR response
                     MAY result if a security layer is already active.  Once a
                     client issues a STLS command, it MUST NOT issue further
                     commands until a server response is seen and the TLS
                     negotiation is complete.

                     The STLS command is only permitted in AUTHORIZATION state
                     and the server remains in AUTHORIZATION state, even if
                     client credentials are supplied during the TLS negotiation.
                     The AUTH command [POP-AUTH] with the EXTERNAL mechanism
                     [SASL] MAY be used to authenticate once TLS client
                     credentials are successfully exchanged, but servers
                     supporting the STLS command are not required to support the
                     EXTERNAL mechanism.

                     Once TLS has been started, the client MUST discard cached
                     information about server capabilities and SHOULD re-issue
                     the CAPA command.  This is necessary to protect against
                     man-in-the-middle attacks which alter the capabilities list
                     prior to STLS.  The server MAY advertise different
                     capabilities after STLS.

                 Possible Responses:
                     +OK -ERR

                 Examples:
                     C: STLS
                     S: +OK Begin TLS negotiation
                     <TLS negotiation, further commands are under TLS layer>
                       ...
                     C: STLS
                     S: -ERR Command not permitted when TLS active
            */

            if (m_SessionRejected)
            {
                this.Write("-ERR Bad sequence of commands: Session rejected.");

                return;
            }
            if (this.IsAuthenticated)
            {
                this.Write("-ERR This ommand is only valid in AUTHORIZATION state (RFC 2595 4).");

                return;
            }
            if (this.IsSecureConnection)
            {
                this.Write("-ERR Bad sequence of commands: Connection is already secure.");

                return;
            }
            if (this.Certificate == null)
            {
                this.Write("-ERR TLS not available: Server has no SSL certificate.");

                return;
            }

            this.Write("+OK Ready to start TLS.");

            try
            {
                SwitchToSecure();

                // Log
                // LogAddText("TLS negotiation completed successfully.");
            }
            catch (System.Exception x)
            {
                // Log
                // LogAddText("TLS negotiation failed: " + x.Message + ".");

                this.Disconnect();
            }
        }

        public Pop3TcpSession(NetCoreServer.TcpServer server)
            : base(server)
        {

            this.commands = new string[] {
                "STLS",
                "USER",
                "PASS",
                "AUTH",
                "STAT",
                "LIST",
                "UIDL",
                "TOP",
                "RETR",
                "DELE",
                "NOOP",
                "RSET",
                "CAPA",
                "QUIT"
            };

            commandHandlers = new System.Collections.Generic.Dictionary<string, foo_t>(System.StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i < commands.Length; ++i)
            {
                commandHandlers.Add(commands[i], foo);
            }

            commandHandlers["QUIT"] = QUIT;
            commandHandlers["CAPA"] = CAPA;
            commandHandlers["NOOP"] = NOOP;
            commandHandlers["USER"] = USER;
            commandHandlers["PASS"] = PASS;
            commandHandlers["STAT"] = STAT;
            commandHandlers["RETR"] = RETR;
            commandHandlers["DELE"] = DELE;
            commandHandlers["RSET"] = RSET; // TODO: A little
            commandHandlers["TOP"] = TOP;   // TODO: A little
            commandHandlers["LIST"] = LIST; // TODO: A little
            commandHandlers["UIDL"] = UIDL; // TODO: A little
            commandHandlers["AUTH"] = AUTH; // TODO: Major ! 
            commandHandlers["STLS"] = STLS; // TODO: HOW ?! 
        }



        protected void Write(string message)
        {
            lock (ColorConsole.LOCK)
            {
                ColorConsole.Log("[SERVER]: Sending message \"");
                ColorConsole.Log(message);
                ColorConsole.LogLine("\\r\\n\"");
            }

            SendAsync(System.Text.Encoding.ASCII.GetBytes(message + "\r\n"));
        }


        protected override void OnConnected()
        {
            ColorConsole.LogLineWithLock($"[SERVER]: TCP session with Id {Id} connected!", System.ConsoleColor.Black, System.ConsoleColor.Yellow);
            

            // Send invite message
            // string message = "Hello from Syslog TCP session ! Please send a message or '!' to disconnect the client!"; SendAsync(message);
            
            string lhn = System.Net.Dns.GetHostName();
            Write("+OK [" + lhn + "] POP3 Service Ready.");
        } // End Sub OnConnected 


        protected override void OnDisconnected()
        {
            ColorConsole.LogLineWithLock($"[SERVER]: TCP session with Id {Id} disconnected!", System.ConsoleColor.Black, System.ConsoleColor.Yellow);
        } // End Sub OnDisconnected 




        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            try
            {
                string message = string.Empty;
                string printMessage = string.Empty;

                // throw new System.Exception("FOOBAR - Fucked up beyond all repair !");


                try
                {
                    int m_BadCommands = 0;

                    message = System.Text.Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
                    if (!string.IsNullOrEmpty(message) && message.Length > 2 && message.EndsWith("\r\n"))
                    {
                        // message.TrimEnd(new char { '\r', '\n' });
                        message = message.Substring(0, message.Length - 2);
                    }
                        

                    printMessage = message.Replace("\r", "\\r").Replace("\n", "\\n");

                    string[] cmd_args = message.Split(new char[] { ' ' }, 2);
                    string cmd = cmd_args[0].ToUpperInvariant();
                    string args = cmd_args.Length == 2 ? cmd_args[1] : "";


                    lock (ColorConsole.LOCK)
                    {
                        ColorConsole.Log("[SERVER]: Received message \"");
                        ColorConsole.Log(printMessage);
                        ColorConsole.LogLine("\".");
                    }



                    if (commandHandlers.ContainsKey(cmd))
                    {
                        this.m_currentCommand = cmd;
                        commandHandlers[cmd](args);
                    }
                    else
                    {
                        m_BadCommands++;
                        // if(m_BadCommands = too_many)
                        // WriteLine("-ERR Too many bad commands, closing transmission channel."); Disconnect();
                        // else
                        this.Write("-ERR Error: command '" + cmd + "' not recognized.");
                    }

                }
                catch (System.Exception e)
                {
                    //a socket error has occured
                    System.Console.WriteLine(e.Message);
                }

                if (message.Length > 0)
                {
                    if (message.IndexOf("QUIT") != -1 && !message.StartsWith("QUIT"))
                    {
                        ColorConsole.LogErrorLineWithLock("[SERVER]: Missed QUIT SIGNAL - Message: \"" + printMessage + "\".");
                    }

                }

            }
            catch (System.Exception ex)
            {
                ColorConsole.LogErrorWithLock(ex);
            }

            // System.Console.WriteLine("exiting message loop");

            // Multicast message to all connected sessions
            // Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            // if (message == "!")
            // Disconnect();

        } // End Sub OnReceived 


        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            ColorConsole.LogErrorLineWithLock($"[SERVER]: TCP session caught an error with code {error}");
        } // End Sub OnError 


    } // End Class SmtpTcpSession 


    // https://stackoverflow.com/questions/16809214/is-smtp-based-on-tcp-or-udp
    public class TcpPop3Server
        : NetCoreServer.TcpServer
    {

        public TcpPop3Server(System.Net.IPAddress address, int port)
            : base(address, port)
        { }


        protected override NetCoreServer.TcpSession CreateSession()
        {
            return new Pop3TcpSession(this);
        } // End Function CreateSession 


        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            ColorConsole.LogErrorLineWithLock($"[SERVER]: TCP server caught an error with code {error}");
        } // End Sub OnError 


        public static void Test()
        {
            // TCP server port
            int port = 110;
            // Port 110 – this is the default POP3 non-encrypted port;
            // Port 995 – this is the port you need to use if you want to connect using POP3 securely.

            System.Console.WriteLine($"TCP server port: {port}");

            System.Console.WriteLine();

            // Create a new TCP Syslog server
            TcpPop3Server server =
                new TcpPop3Server(System.Net.IPAddress.Any, port);

            // Start the server
            System.Console.WriteLine("Server starting...");
            server.Start();
            System.Console.WriteLine("Done!");

            System.Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

            // Perform text input
            while (true)
            {
                System.ConsoleKeyInfo line = System.Console.ReadKey();

                // Restart the server
                if (line.KeyChar == '!')
                {
                    System.Console.WriteLine("Server restarting...");
                    server.Restart();
                    System.Console.WriteLine("Done!");
                    continue;
                } // End if (line == "!") 
                else
                    break;

                // Multicast admin message to all sessions
                // server.Multicast(line);
            } // Next 

            // Stop the server
            System.Console.WriteLine("Server stopping...");
            server.Stop();
            System.Console.WriteLine("Done!");
        } // End Sub Test  


    } // End Class TcpSmtpServer 


} // End Namespace SyslogServer 
