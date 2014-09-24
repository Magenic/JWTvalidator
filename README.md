JWTvalidator
============

Json Web Token (JWT) validation library.

Parses and validates a Json Web Token provided from Azure Mobile Services as a result of AMS having authenticated the user via an identity service such as Facebook, Twitter, etc.

If the token is valid the thread's CurrentPrincipal is set to a Principal/Identity corresponding to the user's identity.

(this code originated in the [MyVote app](https://github.com/Magenic/MyVote)).