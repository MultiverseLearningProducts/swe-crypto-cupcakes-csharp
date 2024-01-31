# SWE Crypto Cupcakes - C#

**See setup instructions on the `main` branch to install dependencies and start the ASP.NET Core server.**

This branch implements a very basic API which allows users to `POST` and `GET`
cupcakes.

A cupcake looks like this

```json
{
  "id": 2,
  "flavor": "Chocolate",
  "instructions": "leave outside for an hour"
}
```

## Coach Notes

The focus on this session is building principled APIs. You will find that the
code in this API is imperfect in many ways, and that is a good thing. The reason
for giving you a codebase up front rather than starting from scratch means we
can model _codebase exploration_ and _refactoring_ - two very important skills
on-the-job.

As you explore the codebase with the apprentices, be sure to point out any of
the features which align with the learning objectives, and make as many
improvements as you wish (but please don't push refactors to the remote
branch!). You could take suggestions from the apprentices, drop hints to see if
they spot some refactors, or take the lead if they seem hesitant.

## Things to see and do


### Folder Structure of .NET App

ASP.NET Core promotes a particular structure and organization of files and folders, so every piece works correctly together. Important functionality here is in the `Controllers`, `Data`, and `Models` directories. `Program.cs` is also a key file that serves as the entry point to this .NET application where the runtime is configured and the server is started.

### Cupcake ID

It could be a chance to talk how we are incrementing the id when creating a new cupcake. Are there other ways to do this?

### Response codes

Have a look at the response codes and see if apprentices recognise any of them.
Have they used these before? Why are they important?

### Error handling and validation

There isn't much! Where might we want to implement some error handling or
validation? Can apprentices suggest any points where errors may occur that
require handling? Anything more we could do to improve validation?

### Make some requests

After completing the setup instructions on the `main` branch and start the server with the `dotnet run --launch-profile https` command, you should be able to hit the API at
`https://localhost:7119`

Try getting a single cupcake with

```bash
curl -L -v -XGET 'https://localhost:7119/cupcakes/3' | json_pp
```

Does it work? What do you see in the request and response? (N.b. in the output
of cURL, anything with a `>` is part of the request, and anything with a `<` is
part of the response.)

Why did we choose `/3` and not `?id=3`? Which is the better implementation? Why?

Try getting many cupcakes with

```bash
curl -L -v -XGET 'https://localhost:7119/cupcakes' | json_pp
```

and

```bash
curl -L -v -XGET 'https://localhost:7119/cupcakes?flavor=Chocolate' | json_pp
```

What other query params could we provide to the users to make their life easier?
(E.g. pagination, sorting).

What design problems are there with this endpoint? (E.g. what if there are two
million rows of data?)

Try creating a cupcake with

```bash
curl -L -v -XPOST \
-H "Content-type: application/json" \
-d '{ "flavor" : "marble", "instructions" : "just heat up and enjoy" }' \
'https://localhost:7119/cupcakes' | json_pp
```

The API sends back the created resource! Why might this be useful for users of
the API?

## Next steps

Apprentices will be given the specification for Snippr, and also some guidance
on which framework aligns with their language. It is their turn to head off and
try to implement the spec for themselves.
