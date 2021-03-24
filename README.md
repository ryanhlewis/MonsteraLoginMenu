<h1 align="center">
Monstera Login Menu
</h1>


![MonsteraLoginMenu](https://dc605.4shared.com/img/4CCm01l1ea/s24/17861e0dd70/monsteralogin?async=&rand=0.7321771691661296)

## Explanation

This is a Login and Register Menu I put together in Unity after roaming around DeviantArt, and running into the concept art down below by [Felipe Viana](https://dribbble.com/felpviana).

I set everything up completely, with working fields, forgot password queries, and even a unique username feature. This all works by using Google's currently free Firebase API, and I took a tutorial and initial codeset for this Login Screen from [XZippyZachX](https://github.com/xzippyzachx).


## Installation

If you want to take this and reform it into whatever your dreams may be, feel free to! Here's some light instructions to get you started.

1. Go to Firebase.com and create an account and set up a free Firebase Project. 

2. Enable Email-Authentication under the Authentication tab, and also enable a Realtime Database with these rules:

``` {
    "rules": {
        ".read": "auth !== null",
        ".write": "auth !== null",
"users": {
  "$uid": {
    ".write": "auth !== null && auth.uid === $uid",
    ".read": "auth !== null && auth.provider === 'password'",
    "username": {
      ".validate": "
        !root.child('usernames').child(newData.val()).exists() ||
        root.child('usernames').child(newData.val()).val() == $uid"
    }
  }
}
   }
}
```

3. Go to Project Settings (Top Left, Gear Icon) and scroll down to the bottom, click Add App (Unity), then fill in anything you want for the App Title and ID (They don't matter!), and finally, download your google-services.json. You'll need it.

4. Place your google-services.json at the root of the Assets directory, and you're good to go! It should all work now- and the code I've written for database unique usernames checks will automatically create the username tables for you. Head over to see it work in action in your Firebase Realtime Database.

5. Enjoy! Go have at it! Rearrange the elements, change the font, the pictures, the logos, the names, everything! 


## Credit Where Credit Is Due

FirebaseAuthTutorial by XZippyZachX: https://github.com/xzippyzachx/UnityFirebaseAuthTutorial

Valorant Login Screen Concept Art by Felipe Viana: https://dribbble.com/shots/11294666-Valorant-Login-Screen-concept-design

Here's the concept art I got the idea for this from-

![Concept Art](https://cdn.dribbble.com/users/4107580/screenshots/11294666/media/67d3a8823f2f6571e239a2cb81f162cf.png)

