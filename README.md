# SWE Crypto Cupcakes - C#

**See setup instructions on the `main` branch to install dependencies and start
the ASP.NET Core server.**

This branch refactors the basic auth flow to use tokens, which allows for a much
improved user experience.

## Coach notes

The big concept here is [tokens](https://mv-swe-docs.netlify.app/backend/tokens.html).
We're using JWTs to demonstrate this.

The framework the apprentice is working with might already have pushed them in
the direction of tokens when they were exploring basic auth last week. This is
fine! The implementation here is quite manual so we can really see what is going
on under the hood.

## Things to see and do


### Create a token secret

This could be anything, but 32 random bytes is a safe bet:

```bash
openssl rand -base64 32
```

produces something like

```bash
M6JHWx2teUqTY5rNnzjgsgWRKtdCqjc5Je+ULdRhqt0=
```

Create your own, or use this one, and save it as a user-secret using .NET's Secret Manager Tool. As a reference for this demo, this token is stored in `appsettings.json`. However, these secrets should usually NOT be committed to the repository, and they should always be stored securely using environment variables or a secret manager.

Set a new secret (e.g. your JwtSettings Secret). You can see it in JSON format in `appsettings.json`.

```bash
dotnet user-secrets set "JwtSettings:Secret" "M6JHWx2teUqTY5rNnzjgsgWRKtdCqjc5Je+ULdRhqt0="
```

View your secrets using:

```bash
dotnet user-secrets list
```

You could also set the JWT token Issuer, Audience, and other settings following the above steps. This demo will only use the token secret for validation.

### Create a user

In this new token based world, we create a user and then sign them in.

```bash
curl -v -XPOST \
-H 'Authorization: Basic dGVzdEB1c2VyLmNvbTpwYXNzd29yZDEyMw==' \
'https://localhost:7119/users' | json_pp
```

creates the user, and

```bash
curl -v -XPOST \
-H 'Authorization: Basic dGVzdEB1c2VyLmNvbTpwYXNzd29yZDEyMw==' \
'https://localhost:7119/users/login' | json_pp
```

signs them in. The latter command should provide you with an accessToken in the
response. Something like:

```bash
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIyIiwiZW1haWwiOiJ0ZXN0QHVzZXIuY29tIiwibmJmIjoxNzA3ODQxMDQ1LCJleHAiOjE3MDg0NDU4NDUsImlhdCI6MTcwNzg0MTA0NX0.5s4X8AM2Pu9cB35qHIzEMfyb2h_Q8gx2GQUNiM3j4LU
```

If you were to sign in again, you would get a different token each time.

### Access a resource

The GET `'/cupcakes'` endpoint has been protected by requiring a JWT. In order to access them,
you don't need to send your password again, but instead you send your token!

```bash
curl -L -v -XGET 'https:///localhost:7119/cupcakes'
```

will fail, but

```bash
curl -L -v -XGET \
-H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIyIiwiZW1haWwiOiJ0ZXN0QHVzZXIuY29tIiwibmJmIjoxNzA3ODQxMDQ1LCJleHAiOjE3MDg0NDU4NDUsImlhdCI6MTcwNzg0MTA0NX0.5s4X8AM2Pu9cB35qHIzEMfyb2h_Q8gx2GQUNiM3j4LU' \
'https:///localhost:7119/cupcakes' | json_pp
```

should succeed.

Try changing a single character in the token and see what happens. This prevents
hackers from spoofing tokens.

If you split the token at the `'.'` characters, you can decode the parts from
Base 64 to utf-8 so that you can see what they contain.

#### More about the token secret

Option 1: In the [jwt.io](https://jwt.io) sandbox, paste the full token, and enter a different token secret in the `your-256-bit-secret` text box in the "Verify Signature" section to re-sign it. Use the new token in your Authorization header to try to access cupcake info using the request above - it should fail because the secret we use to check the token must be the same secret we used to sign it. Paste in the original token secret into this secret text box, and retry the token - it should now work!

Option 2: You could also accomplish the same thing by stopping your server and restarting it using .NET's Hot Reload feature - this will allow you to make changes to the server in real time:

```bash
    dotnet watch run --launch-profile https
```

Once you have it up and running again, repeat the step [above](#create-a-user) to create the user. Then, in `IdentityService.cs`, change the `_jwtSettings.Secret` reference used in `CheckToken()` that is being assigned to `IssuerSigningKey` to any random string of characters. After that, try to access the cupcakes resource (GET `'/cupcakes'` endpoint) with the previously generated token in the Authentication header - it won't work! Change the random string back to `_jwtSettings.Secret` and it will now succeed.

### UserController.cs

Check out the new POST `'/users/login'` endpoint. This is calling our `GenerateToken()` function in `IdentityService.cs` to generate and send a JWT back to the client.

### JwtMiddleware.cs

This is the middleware which implements authorization using JWTs, parsing out the token, and then checking its validity using the `CheckToken()` function in `IdentityService.cs`.

Depending on the response, it will either store the validated user in HttpContext or send back an error message.

### IdentityService.cs

The `GenerateToken()` function takes in the user and creates a token that is signed with our `_jwtSettings.Secret` and sends it back.

`CheckToken()` validates whether the token was signed with the `_jwtSettings.Secret`, verifying that it really came from our server. Once verified, it will send back user information with the ClaimsPrincipal.

Have a look at these functions. At what points do we generate the token and check the token? What error handling is present?

The new `JwtSettings.cs` model is also used here and in `Program.cs` to hold JWT configuration settings.

## Next steps

As mentioned, the frameworks the apprentices are using might implement
token-based authentication in very different ways, and they might not need to do
much to handle the verification of tokens and handling of secrets. The
underlying protocol should be roughly the same, however, so encourage them to
lean into their framework's documentation and not be too worried if the
implementation of auth looks quite different.

Focus on the requirements in the spec.