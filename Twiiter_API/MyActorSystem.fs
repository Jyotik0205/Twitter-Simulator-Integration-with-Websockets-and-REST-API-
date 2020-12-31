namespace MyWebApi

open System
open Akka.Actor
open Akka.FSharp
open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Data
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open MyWebApi
open Akka.Actor
open Akka.FSharp
type Tmessage=
        |Readtweet of string
type Wmesage=
        |Write of User  
module UserTable=
    let dt = new DataTable()
    let dc1 = new DataColumn("ID")
    let dc2 = new DataColumn("F_NAME")
    let dc3 = new DataColumn("L_NAME")
    let dc4 = new DataColumn("password")
    dt.Columns.Add(dc1)
    dt.Columns.Add(dc2)
    dt.Columns.Add(dc3)
    dt.Columns.Add(dc4)
    let subs_master = new DataTable()
    let subs_col1 = new DataColumn("subscriberId")
    let subs_col2 = new DataColumn("subscribee")
    subs_master.Columns.Add(subs_col1)
    subs_master.Columns.Add(subs_col2)
    let tweet_master=new DataTable()
    let tweet_col1 = new DataColumn("From")
    let tweet_col2 = new DataColumn("Mention")
    let tweet_col3 = new DataColumn("Hashtag")
    let tweet_col4 = new DataColumn("Message")
    tweet_master.Columns.Add(tweet_col1)
    tweet_master.Columns.Add(tweet_col2)
    tweet_master.Columns.Add(tweet_col3)
    tweet_master.Columns.Add(tweet_col4)
module MyActorSystem=
     let spawn_tweet_reader system name=
              spawn system ("R_Actor"+string(name))<|
                      fun mailbox->
                          let rec loop()=
                                actor{
                                       let! msg=mailbox.Receive()
                                       match msg with
                                       |Readtweet qrystr-> printfn ""                           
                                          
                                       return!loop()
                                }
                          loop()
     let spawn_user_writer system name=
              spawn system ("W_Actor"+string(name))<|
                      fun mailbox->
                          let rec loop()=
                                actor{
                                       let! msg=mailbox.Receive()
                                       match msg with
                                       |Write user-> UserTable.dt.Rows.Add(user.Id,user.Fname,user.Lname,user.Password)|>ignore
                                                                         
                                          
                                       return!loop()
                                }
                          loop()
     type Mainsystem(typeo:string, noa:int)=
            let sys=ActorSystem.Create(typeo)
            let a=spawn_user_writer sys noa|>ignore
            member __.Getactsystem(): ActorSystem=
                 sys