Component Title - Link Preview 

Getting Started -

This component provides the meta tag information for the URL you have entered. We have created a class library LinkPreview and in this library we have a method GetMetaTagValueUrl which takes string type URL and fetches all the information related to the URL and returns in MetaTagModel format.
To use this class library we have created a console application for showing the demo. 
Below are the steps to describe how we can use it in our applications.
1.	Add reference of LinkPreview.dll in your project.
2.	Add HtmlAgilityPack from Nuget.
3.	Create object of LinkPreview class and call its method GetMetaTagValueUrl by passing URL to it.
4.	In response you will get the MetaTagModel key value pair.
Example
Input :  https://www.youtube.com/watch?v=1EnvkPf7t6Y
Output :
{"Title":null,"ImageUrl":"https://i.ytimg.com/vi/1EnvkPf7t6Y/hqdefault.jpg","IsVideoUrl":true,"DomainName":"www.youtube.com","Url":"https://www.youtube.com/watch?v=1EnvkPf7t6Y","VideoId":"1EnvkPf7t6Y","Description":null}

