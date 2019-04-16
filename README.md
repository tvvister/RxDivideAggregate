### Welcome to the RxDivideAggregate wiki!
This page is about some problem you can encounter and solution for it.  
Let's start with simple example:  
So imagine, you have **Observable\<FireStates\>**, where **FireStates** is thoughtful c# type for information about hundreds fire detectors states in your country. Suggest the infomation system has some features:
1. In every FireStates there is may information about only several fires. In other words you have dynamic count states in Observable element.
2. You can't rely on FireStates totally. It has some mistakes. But if two consecutive reports of some **fireId** are the same you will make a conclusion with 100% truth.  
     Obs :S------------D------I--D--D  
     time:1------------2------3--4--5  
     Res :--------------------------D  

Diagramm:
![](https://drive.google.com/uc?id=1GlLmRBEHIUNN9JSZFUP1JrSzTAIX_ilc)
