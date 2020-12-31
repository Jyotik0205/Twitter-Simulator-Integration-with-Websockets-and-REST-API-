#r @"bin\MCD\Debug\netcoreapp3.1\Hopac.dll"
#r @"bin\MCD\Debug\netcoreapp3.1\Hopac.Core.dll"
#r @"bin\MCD\Debug\netcoreapp3.1\HttpFs.dll"
#r @"bin\MCD\Debug\netcoreapp3.1\Fsharp.Core.dll"
#r @"C:\Users\jyotik\.nuget\packages\fsharp.data\3.3.3\lib\net45\Fsharp.Data.DesignTime.dll"
#r @"bin\MCD\Debug\netcoreapp3.1\Fsharp.Data.dll"
#r @"bin\MCD\Debug\netcoreapp3.1\Akka.dll"
#r @"bin\MCD\Debug\netcoreapp3.1\Akka.Fsharp.dll"
#r @"bin\MCD\Debug\netcoreapp3.1\Fsharp.Data.Json.dll"


open HttpFs.Client
open Hopac
open System
open System.Web // TODO: remember to add reference to project ->
open System.Text
open FSharp.Data
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open FSharp.Data.Json

// let numNodes=10000

// let system=ActorSystem.Create("tweet")
// type actormessage=
//     |Register
//     |Subscribe of int
//     |Connect
//     |Tweet of string*string*string
//     |Retweet of string*string*string
// type ParentMessage=
//     |Start
//     |ZipfSubs
// type ApiHttpResponse =
//     | Ok of body:string
//     | Error of statusCode:int
//     | Exception of e:exn
let postrequest url requeststr=
    let request =
        Request.createUrl Post url
        |> Request.body (BodyString requeststr)
        |> Request.setHeader (Referer "https://github.com/haf/Http.fs")
        |> Request.setHeader (UserAgent "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0")
        |> Request.setHeader (Accept "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
       // |> Request.setHeader (AcceptLanguage "Accept-Language: en-US")
       // |> Request.setHeader (ContentType (ContentType.create("application", "x-www-form-urlencoded")))
        |> Request.setHeader (ContentType (ContentType.create("application", "json")))
        |> Request.setHeader (Custom ("DNT", "1"))
        |> Request.responseCharacterEncoding Encoding.UTF8
        |>getResponse
        |>Job.bind Response.readBodyAsString
    request|>run

let getrequest url=
    let request =
        Request.createUrl Get url
        |> Request.setHeader (Referer "https://github.com/haf/Http.fs")
        |> Request.setHeader (UserAgent "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0")
        |> Request.setHeader (Accept "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
       // |> Request.setHeader (AcceptLanguage "Accept-Language: en-US")
       // |> Request.setHeader (ContentType (ContentType.create("application", "x-www-form-urlencoded")))
        |> Request.setHeader (ContentType (ContentType.create("application", "json")))
        |> Request.setHeader (Custom ("DNT", "1"))
        |> Request.responseCharacterEncoding Encoding.UTF8
        |>getResponse
        |>Job.bind Response.readBodyAsString
    request|>run
// JsonValue
// type Simple = new FSharp.Data.JsonParser();
// let simple = Simple.Parse(""" { "name":"Tomas", "age":4 } """)
// let spawn_printer system name=
//               spawn system ("Actor"+string(name))<|
//                       fun mailbox->
//                           let mutable loggedin=false;
//                           let id=name;
//                           let password="random";
//                           let rec loop()=
//                                 actor{
//                                        let! msg=mailbox.Receive()
//                                        match msg with
//                                        |Register ->                           let requeststring="""{ "Id":" """+string(name) + """", "Fname": "Daye","Lname":"Kim", "Password":"random"}"""
//                                                                               postrequest "http://localhost:5000/weatherforecast" requeststring|>ignore
//                                        |Subscribe k->                         let str1="""{ "subscriberId":" """+string(id)
//                                                                               let str2= """","subscribee":" """+string(k) +  """"}"""
//                                                                               let requeststring=str1+str2;
//                                                                               postrequest "http://localhost:5000/weatherforecast/subscribe" requeststring|>ignore
//                                                                               //printfn "StatusCode: %s" resp
//                                        |Connect ->                            printfn "Login"
//                                        |Tweet (mention,hashtag,message) ->    let str1="""{ "From":" """+string(id)
//                                                                               let str2="""","Mention":" """+mention
//                                                                               let str3="""","Hashtag":" """+hashtag
//                                                                               let str4= """","Message":" """+message +  """"}"""
//                                                                               let requeststring=str1+str2+str3+str4;  
//                                                                               let resp=postrequest "http://localhost:5000/weatherforecast/tweet" requeststring 
//                                                                               printfn "StatusCode: %s" resp
//                                         |Retweet (mention,hashtag,message)->  let str1="""{ "From":" """+string(id)
//                                                                               let str2="""","Mention":" """+mention
//                                                                               let str3="""","Hashtag":" """+hashtag
//                                                                               let str4= """","Message":" """+message +  """"}"""
//                                                                               let requeststring=str1+str2+str3+str4;  
//                                                                               let resp=postrequest "http://localhost:5000/weatherforecast/retweet" requeststring 
//                                                                               printfn "StatusCode: %s" resp      
//                                        return!loop()
//                                 }
//                           loop()
// let info =
//   FSharp.Data.JsonValue.Parse(resp2)
       
        
// type Subscribe = JsonProvider("{ subscriberId: string
//       subscribee: string
//     }")
   
// Subscribe.Parse(info)
// makeuser 100
// let id=4
// let mention="34"
// let hashtag="45"
// let message="dlfiugndfn"
// let resp=getrequest "http://localhost:5000/weatherforecast/search/mention/ 34" 
// let resp1=getrequest "http://localhost:5000/weatherforecast/search/hashtag/ 45" 
// let resp2=getrequest "http://localhost:5000/weatherforecast/subscribes/ 18" 
// let resp3=getrequest "http://localhost:5000/weatherforecast/search/subscribes/ 18" 

// printfn "Response: %s" resp3
// printfn "Jsonvalue"
// let k12=info.[0]
// k12.[1]
// let items = Array(info)
// for item in info do
//  printf "%s " item?subscribee.AsString
// let controllerActor= spawn system "parent" <| fun mailbox->
              
//                let ActorList =  
//                    [1..numNodes]
//                       |> List.map(fun id-> spawn_printer mailbox id)
              
//                let rec loop()=
//                    actor{
//                    let! msg=mailbox.Receive()
//                   // printfn "From Parent %A" msg
//                    match msg with
//                    |Start->       //printfn "Received Message"
//                                   for i=1 to numNodes do
//                                       let childsender=system.ActorSelection("akka://tweet/user/parent/"+"Actor"+string(i))
//                                       childsender.Tell(Register)
//                    |ZipfSubs->    for i=1 to numNodes do 
//                                       let childsender=system.ActorSelection("akka://tweet/user/parent/"+"Actor"+string(i))
//                                       for j=1 to (numNodes-1)/i  do
//                                        childsender.Tell(Subscribe j)
//                                   return()
//                    return! loop()
                    

//                    }
//                loop()


// let parent1=system.ActorSelection("akka://tweet/user/parent")
// printfn "Press Enter..."
// Console.ReadLine()|>ignore
// parent1.Tell(Start)
// let resp=getrequest "http://localhost:5000/weatherforecast" 
// printfn "Total request Received: %s" resp
// Console.ReadLine()|>ignore
// parent1.Tell(ZipfSubs)
//Register
// let requeststring="""{ "Id":" """+string(6) + """", "Fname": "Daye","Lname":"Kim", "Password":"random"}"""
// postrequest "http://localhost:5000/weatherforecast" requeststring|>ignore
// let requeststring1="""{ "Id":" """+string(8) + """", "Fname": "Daye","Lname":"Kim", "Password":"random"}"""
// postrequest "http://localhost:5000/weatherforecast" requeststring1|>ignore
// let requeststring2="""{ "Id":" """+string(10) + """", "Fname": "Daye","Lname":"Kim", "Password":"random"}"""
// postrequest "http://localhost:5000/weatherforecast" requeststring2|>ignore

// let str1="""{ "subscriberId":" """+string(10)
// let str2= """","subscribee":" """+string(6) +  """"}"""
// let requeststring3=str1+str2;
// postrequest "http://localhost:5000/weatherforecast/subscribe" requeststring3|>ignore
// let resp1=getrequest "http://localhost:5000/weatherforecast/" 
// let resp2=getrequest "http://localhost:5000/weatherforecast/subscribes/ 8" 
//Tweet
// let mention="4"
// let hashtag="travel"
// let message="Wow! This Restraunt is so awesome!"
// let str1="""{ "From":" """+string(6)
// let str2="""","Mention":" """+mention
// let str3="""","Hashtag":" """+hashtag
// let str4= """","Message":" """+message +  """"}"""
// let requeststring=str1+str2+str3+str4;  
// let resp3=postrequest "http://localhost:5000/weatherforecast/tweet" requeststring 
// printfn "StatusCode: %s" resp

// let resp=getrequest "http://localhost:5000/weatherforecast/search/mention/ 8" 
// let resp1=getrequest "http://localhost:5000/weatherforecast/search/hashtag/ food" 
// let resp2=getrequest "http://localhost:5000/weatherforecast/search/subscribes/ 10" 


//printfn "%A" resp2
//let resp1=getrequest "http://localhost:5000/weatherforecast/" 
let mutable flag=true
let mutable input=""
let ht="food"
while flag do
  printfn"Press 1.For Registration 2.For Posting Tweet 3. For Searching 4.Subscribe to an User"
  input<-Console.ReadLine()
  if input="1" then
    printfn "Enter Id:"
    let a=Console.ReadLine()
    printfn "Enter Password:"
    let pass=Console.ReadLine()
    let requeststring="""{ "Id":" """+a + """", "Fname": "Daye","Lname":"Kim", "Password":"random"}"""
    postrequest "http://localhost:8000/weatherforecast" requeststring|>ignore
  if input="2" then
    printfn "Enter Your ID:"
    let id=Console.ReadLine()
    printfn "Enter Message:"
    let message=Console.ReadLine()
    printfn "Enter Mentions:"
    let ment=Console.ReadLine()
    printfn "Enter HashTag:"
    let ht=Console.ReadLine()
    let str1="""{ "From":" """+id
    let str2="""","Mention":" """+ment
    let str3="""","Hashtag":" """+ht
    let str4= """","Message":" """+message +  """"}"""
    let requeststring=str1+str2+str3+str4;  
    let resp3=postrequest "http://localhost:8000/weatherforecast/tweet" requeststring 
    printfn "Tweet Posted"
  if input="3" then
    printfn "Press 1. For Search by Mention 2. Search by HashTag 3.Search Tweets of Subscribers"
    let subInput=Console.ReadLine()
    if subInput="1" then
        printfn "Enter the mention you want to search on:"
        let mention=Console.ReadLine()
        let resp=getrequest ("http://localhost:8000/weatherforecast/search/mention/ "+mention)
        printfn "Tweets are: %A" resp
    if subInput="2" then
        printfn "Enter the HashTag you want to search on:"
        let ht=Console.ReadLine()
        let resp=getrequest ("http://localhost:8000/weatherforecast/search/hashtag/ "+ht )
        printfn "Tweets are: %A" resp
    if subInput="3" then
        printfn "Enter your ID:"
        let ht=Console.ReadLine()
        let resp=getrequest ("http://localhost:8000/weatherforecast/search/subscribes/ "+ht )
        printfn "Tweets are: %A" resp
  if input="4" then
     printfn "Enter Your Id:"
     let id=Console.ReadLine()
     printfn "Enter Your Id You want to subscribe:"
     let sid=Console.ReadLine()
     let str1="""{ "subscriberId":" """+id
     let str2= """","subscribee":" """+sid +  """"}"""
     let requeststring3=str1+str2;
     postrequest "http://localhost:8000/weatherforecast/subscribe" requeststring3|>ignore
     printfn "Now you are subscribing to: %s" sid
