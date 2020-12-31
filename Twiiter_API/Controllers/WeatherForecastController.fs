namespace MyWebApi.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Collections.Concurrent
open System.Data
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open MyWebApi
open Akka.Actor
open FSharp.Json
open Akka.FSharp
open Suave
open Suave.Http
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Files
open Suave.RequestErrors
open Suave.Logging
open Suave.Utils
open FSharp.Core
open System
open System.Net
open System.Threading
open Suave.Sockets
open Suave.Sockets.Control
open Suave.WebSocket
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
    let tweetconcHash=new ConcurrentDictionary<string,Collections.Generic.List<Tweet>>();
    let tweetconcMent=new ConcurrentDictionary<string,Collections.Generic.List<Tweet>>();
    let tweetconcFrom=new ConcurrentDictionary<string,Collections.Generic.List<Tweet>>();
    let usertable=new ConcurrentDictionary<string,string>();
    let subsicriptionTable=new ConcurrentDictionary<string,Collections.Generic.List<string>>();
    let tweetsbyid= new ConcurrentDictionary<string,Tweet>();
    //let subsicriptionTable=new System.Collections.Generic.List<System.Collections.Generic.List<string>>();
    let mutable c=0
    let mutable tcount=0
    for i=1 to 100|>int do
                   let subs=new System.Collections.Generic.List<string>()
                   for j=1 to (100-1)/i do 
                      subs.Add(string(j))
                   subsicriptionTable.TryAdd(string(i),subs)|>ignore

module Mywebsocket=
  type MessageType = {
          action: string
          data: string
  }


  type ServerCommand = 
        | InitialiseServer 
  type amessge=
    |Show of string

// global variables
  let system1 = System.create "twitterServer" (Configuration.defaultConfig())
  let serverActor (mailbox: Actor<_>) =
            let rec loop () = actor {
                let! message = mailbox.Receive ()
                let sender = mailbox.Sender()
                //let requestObject = Json.deserialize<MessageType> message    
                match message with 
                |Show id-> return()
                
                |_ ->
                  printfn "Error in Request"
                return! loop ()
            }
            loop ()



  let twitterServer = spawn system1 "server" serverActor


  let rec sendmessage (webSocket:WebSocket,id:string) =
      async {
          // let responsedata =UserTable.tweetconcFrom.TryGetValue(id)
          // let (b,resp)=responsedata
          // let ans=resp.ToArray()
          let k2=UserTable.subsicriptionTable.TryGetValue(id)
          let (a,slist)=k2
          printfn "Searching For %s" id
          let ans=new List<Tweet>()
          for subs in slist do
          // let qstr=String.Format("From = '{0}'",subs);
              printfn "query string is %s" subs
              let k=UserTable.tweetconcFrom.TryGetValue(subs)
              let (a,b)=k
              printfn "Size of tweet %i" b.Count
              for i=1 to b.Count do
                ans.Add(b.[i-1])
          let answ=ans.ToArray()
          let byteResponse =
            (Json.serialize answ)
            |> System.Text.Encoding.ASCII.GetBytes
            |> ByteSegment
          let! task = webSocket.send Text byteResponse true
          do! Async.Sleep(5000)
          
          return! sendmessage(webSocket,id)

      }
  // let get(id:string)=
  //   async{
  //     let responsedata =UserTable.tweetconcFrom.TryGetValue(id)
  //     let (a,b)=responsedata
  //     return! UserTable.tweetconcFrom.TryGetValue(id)
  //   }
  let ws (webSocket : WebSocket) (context: HttpContext) =
    socket {
      let mutable loop = true
  
      printfn "Socket Connected - Tweet Feed Is On"
  
      let cts = new CancellationTokenSource()
      
      while loop do
        let! msg = webSocket.read()
        match msg with
        | (Text, data, true) ->
          let command = UTF8.toString data
          printfn "Request : %A" command
         // let task = twitterServer <? (Show command)
          Async.Start(sendmessage(webSocket,command), cancellationToken = cts.Token)
          //let response = Async.RunSynchronously (task, 1000)
         // let reponse=get(command)|>Async.RunSynchronously
          let reponse="Logged IN"
          let byteResponse =
            reponse
            |> System.Text.Encoding.ASCII.GetBytes
            |> ByteSegment 
  
          do! webSocket.send Text byteResponse true
  
        | (Close, _, _) ->
          printfn "C%A" Close
          let emptyResponse = [||] |> ByteSegment
          do! webSocket.send Close emptyResponse true
          loop <- false
  
        | _ -> printfn "Matched Nothing %A" msg
      }
                      
                      /// An example of explictly fetching websocket errors and handling them in your codebase.
  let wsWithErrorHandling (webSocket : WebSocket) (context: HttpContext) = 
                         
                         let exampleDisposableResource = { new IDisposable with member __.Dispose() = printfn "Resource needed by websocket connection disposed" }
                         let websocketWorkflow = ws webSocket context
                         
                         async {
                          let! successOrError = websocketWorkflow
                          match successOrError with
                          // Success case
                          | Choice1Of2() -> ()
                          // Error case
                          | Choice2Of2(error) ->
                              // Example error handling logic here
                              printfn "Error: [%A]" error
                              exampleDisposableResource.Dispose()
                              
                          return successOrError
                         }
                      
                   
                      
                     
                   // startWebServer { defaultConfig with logger = Targets.create Verbose [||] } app
              
            
type Tmessage=
        |Readtweet of string
type Wmesage=
        |Write of User
        |Subs of Subscribe
        |ATweet of Tweet  
[<ApiController>]
[<Route("[controller]")>]
type WeatherForecastController (logger : ILogger<WeatherForecastController>) =
    inherit ControllerBase()
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
                                       |Write user-> if not(UserTable.usertable.ContainsKey(user.Id)) then
                                                         while not(UserTable.usertable.TryAdd(user.Id,user.Password)) do
                                                              // printfn "OOps"
                                                               Async.Sleep(0)|>ignore
                                       |Subs data->         
                                                               if UserTable.subsicriptionTable.ContainsKey(data.subscriberId) then
                                                                    let l1=UserTable.subsicriptionTable.TryGetValue(data.subscriberId)
                                                                    let (k,b)=l1;
                                                                    //printfn "Boolean:%b and List:%A" k b
                                                                    
                                                                    b.Add(data.subscribee)
                                                                    while not(UserTable.subsicriptionTable.TryUpdate(data.subscriberId,b,b)) do
                                                                       //printfn "OOps"
                                                                       Async.Sleep(0)|>ignore
                                                                //printfn "After first If:%d" tweetconcHash.Count
                                                                else
                                                                   let ltemp=new Collections.Generic.List<string>()
                                                                   ltemp.Add(data.subscribee)
                                                                   while not(UserTable.subsicriptionTable.TryAdd(data.subscriberId,ltemp)) do
                                                                       // printfn "OOps"
                                                                        Async.Sleep(0)|>ignore
                                                                         
                                       |ATweet data->        UserTable.tcount<-UserTable.tcount+1
                                                             printfn "tcount:%i"UserTable.tcount
                                                             let temp:Tweet={  From= data.From
                                                                               Mention= data.Mention
                                                                               Hashtag= data.Hashtag
                                                                               Message= data.Message
                                                                               id=" "+string(UserTable.tcount)
                                                                              }
                                                             printfn "temp.id:%s"temp.id
                                                             while not(UserTable.tweetsbyid.TryAdd(temp.id,temp)) do
                                                                 Async.Sleep(0)|>ignore
                                                             if UserTable.tweetconcHash.ContainsKey(data.Hashtag) then
                                                                   let l1=UserTable.tweetconcHash.TryGetValue(data.Hashtag)
                                                                   let (k,b)=l1;
                                                                   //printfn "Boolean:%b and List:%A" k b
                                                                   
                                                                   b.Add(temp)
                                                                   while not(UserTable.tweetconcHash.TryUpdate(data.Hashtag,b,b)) do
                                                                      //printfn "OOps"
                                                                      Async.Sleep(0)|>ignore
                                                                   //printfn "After first If:%d" tweetconcHash.Count
                                                             else
                                                                  let ltemp=new Collections.Generic.List<Tweet>()
                                                                  ltemp.Add(temp)
                                                                  while not(UserTable.tweetconcHash.TryAdd(data.Hashtag,ltemp)) do
                                                                      // printfn "OOps"
                                                                       Async.Sleep(0)|>ignore
                                                                 // printfn "After first else:%d" tweetconcHash.Count

                                                             if UserTable.tweetconcMent.ContainsKey(data.Mention) then
                                                                   let l1=UserTable.tweetconcMent.TryGetValue(data.Mention)
                                                                   let (k,b)=l1;
                                                                   b.Add(temp)
                                                                   while not(UserTable.tweetconcMent.TryUpdate(data.Mention,b,b)) do
                                                                      Async.Sleep(0)|>ignore
                                                                  // printfn "After second if:%d" tweetconcMent.Count
                                                             else
                                                                  let ltemp=new Collections.Generic.List<Tweet>()
                                                                  ltemp.Add(temp)
                                                                  while not(UserTable.tweetconcMent.TryAdd(data.Mention,ltemp)) do
                                                                       Async.Sleep(0)|>ignore
                                                                  //printfn "After second else:%d" tweetconcMent.Count
                                                             if UserTable.tweetconcFrom.ContainsKey(data.From) then
                                                                   let l1=UserTable.tweetconcFrom.TryGetValue(data.From)
                                                                   let (k,b)=l1;
                                                                   b.Add(temp)
                                                                   while not(UserTable.tweetconcFrom.TryUpdate(data.From,b,b)) do
                                                                      Async.Sleep(0)|>ignore
                                                                   //printfn "After third if:%d" tweetconcFrom.Count
                                                             else
                                                                  let ltemp=new Collections.Generic.List<Tweet>()
                                                                  ltemp.Add(temp)
                                                                  while not(UserTable.tweetconcFrom.TryAdd(data.From,ltemp)) do
                                                                       Async.Sleep(0)|>ignore
                                                                 
                                                             return()                        
                                          
                                       return!loop()
                                }
                          loop()
    
    let sys=ActorSystem.Create("Writer")
    let a=spawn_user_writer sys 1|>ignore
            
    
    [<HttpGet>]
    member __.Get() : int =
        UserTable.usertable.Count
    [<Route("sno")>]    
    [<HttpGet>]
    member __.Getsno() : int =
        UserTable.dt.Rows.Count
    [<Route("search/mention/{id}")>]
    [<HttpGet>]
    member __.Get(id :string) : List<Tweet> =
       printfn "Searching For %s" id
       let a=UserTable.tweetconcMent.TryGetValue(id)
       let (k,b)=a;
       b
    [<Route("search/hashtag/{id}")>]
    [<HttpGet>]
    member __.GetList(id :string) : List<Tweet> =
        printfn "Searching For %s" id
        let a=UserTable.tweetconcHash.TryGetValue(id) 
        let (k,b)=a;
        b
    [<Route("subscribes/{id}")>]
    [<HttpGet>]
    member __.GetsubsList(id :string) : List<string> =
       printfn "Searching For %s" id
       let qstr=String.Format("subscribee = '{0}'",id);
       let k=UserTable.subsicriptionTable.TryGetValue(id)
       let (a,l)=k
       l
      
    [<Route("search/subscribes/{id}")>]
    [<HttpGet>]
    member __.GetsubstweetList(id :string) : List<Tweet> =
       let slist=__.GetsubsList(id) 
       let mutable a=[]
       printfn "Searching For %s" id
       let ans=new List<Tweet>()
       for subs in slist do
        // let qstr=String.Format("From = '{0}'",subs);
            printfn "query string is %s" subs
            let k=UserTable.tweetconcFrom.TryGetValue(subs)
           
            let (a,b)=k
            printfn "Size of tweet %i" b.Count
            for i=1 to b.Count do
              ans.Add(b.[i-1])
         
       ans    
     [<HttpPost>]
     member _.Post(data: User): int=
          let writer=sys.ActorSelection("akka://Writer/user/W_Actor1")
          writer.Tell(Write data);
          0
     [<Route("subscribe")>]
     [<HttpPost>]
     member _.Post(data: Subscribe): int=
      // DataRow workRow = UserTable.dt.NewRow(); 
        let writer=sys.ActorSelection("akka://Writer/user/W_Actor1")
        writer.Tell(Subs data);
        0
     [<Route("tweet")>]
     [<HttpPost>]
     member _.Post(data: Tweet): int=
      // DataRow workRow = UserTable.dt.NewRow(); 
      let writer=sys.ActorSelection("akka://Writer/user/W_Actor1")
      writer.Tell(ATweet data);
      0
       
     [<Route("retweet")>]  
     [<HttpPost>]
     member _.ReTweet(data: ReTweet): int=
      // DataRow workRow = UserTable.dt.NewRow(); 
      printfn "Id is:%s" data.id
      printfn "%A" data
      let resp=UserTable.tweetsbyid.TryGetValue(data.id)
      let (a,j)=resp
      let temp:Tweet={ From= data.From
                       Mention= j.Mention
                       Hashtag= j.Hashtag
                       Message= "Retweeted as:"+ j.Message
                       id=null
                       }
      
      let writer=sys.ActorSelection("akka://Writer/user/W_Actor1")
      writer.Tell(ATweet temp);
      0