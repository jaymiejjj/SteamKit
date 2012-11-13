﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SteamKit2;

namespace Sample1_Logon
{
    class Program
    {
        static SteamClient steamClient;

        static SteamUser steamUser;

        static bool isRunning;

        static string user, pass;


        static void Main( string[] args )
        {
            if ( args.Length < 2 )
            {
                Console.WriteLine( "Sample1: No username and password specified!" );
                return;
            }

            // save our logon details
            user = args[ 0 ];
            pass = args[ 1 ];

            // create our steamclient instance
            steamClient = new SteamClient();

            // get the steamuser handler, which is used for logging on after successfully connecting
            steamUser = steamClient.GetHandler<SteamUser>();

            Console.WriteLine( "Connecting to Steam..." );

            // initiate the connection
            steamClient.Connect();

            // create our callback handling loop
            while ( isRunning )
            {
                // wait for a callback to be posted
                var callback = steamClient.WaitForCallback( true );

                // handle the callback
                // the Handle function will only call the passed in handler
                // if the callback type matches the generic type
                callback.Handle<SteamClient.ConnectedCallback>( c =>
                {
                    if ( c.Result != EResult.OK )
                    {
                        Console.WriteLine( "Unable to connect to Steam: {0}", c.Result );

                        isRunning = false;
                        return;
                    }

                    Console.WriteLine( "Connected to Steam! Logging in '{0}'...", user );

                    steamUser.LogOn( new SteamUser.LogOnDetails
                    {
                        Username = user,
                        Password = pass,
                    } );
                } );

                callback.Handle<SteamClient.DisconnectedCallback>( c =>
                {
                    Console.WriteLine( "Disconnected from Steam" );

                    isRunning = false;
                } );

                callback.Handle<SteamUser.LoggedOnCallback>( c =>
                {
                    if ( c.Result != EResult.OK )
                    {
                        Console.WriteLine( "Unable to logon to Steam: {0} / {1}", c.Result, c.ExtendedResult );

                        isRunning = false;
                        return;
                    }

                    Console.WriteLine( "Successfully logged on!" );

                    // at this point, we'd be able to perform actions on Steam
                    // such as go online on friends, send chat messages, join chat rooms, request app info, etc

                    // for this sample we'll just log off
                    steamUser.LogOff();
                } );

                callback.Handle<SteamUser.LoggedOffCallback>( c =>
                {
                    Console.WriteLine( "Logged off of Steam: {0}", c.Result );
                } );
            }
        }
    }
}