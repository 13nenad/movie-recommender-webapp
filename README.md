## MovieRecommender

Purpose of the website was to help users find a movie they would like by providing filtering and searching capabilities, as well as providing recommendations. All movie details, user ratings and recommendation data are stored in a CosmosDB. To obtain recommendations a machine learning (Collaborative Filtering) technique is used through a DataBricks job leveraging a Spark cluster and the Spark framework. More specifically, an Alternating Least Squares (ALS) algorithm is used and the notebooks used can be found here: https://github.com/13nenad/movie-recommender-notebooks

### Data
Initial data was obtained from https://grouplens.org/datasets/movielens/
The small dataset was used which contains: 100,000 ratings applied to 9,000 movies by 600 users. Last updated 9/2018.

### Webapp
- The webapp is hosted as an Azure Webapp Service and can be found here: https://nenad-movierecommender.azurewebsites.net
- To have a play with it you can use the userId=325 on the login screen
- ASP.NET Core using .NET SDK 6.0 (C#)
- MVC pattern using Razor pages
- Published using GitHub Actions (publish is triggered upon push to master)
 
### Functionality/Skills incorporated:
- Queries, inserts and deletes data from a distributed data storage (CosmosDB)
- Filtering by genre
- Search bar text search
- Search bar suggestions after 3 entered characters
- Carousel automatic sliding and manual left/right slide
- Rating widget:
	- Can add/update/delete ratings
- Force Run Recommendations menu item:
	- Used for demonstration purposes, to demonstrate recommendation updates (after ratings some movies)
	- Uses the DataBricks API to trigger a job which runs a notebook
	- The notebook recalculates all of the predictions and only updates the current user's recommendations in CosmosDB (it does not update all user's ratings as that is more time consuming)
	- Note that this functionailty will only work if the cluster is active, and to save money it is inactive by default
- CI/CD pipeline (GitHub)

### Architecture

![image](https://user-images.githubusercontent.com/5523082/162183282-08749ef2-5f5c-4a43-8fbc-f161a5fb4435.png)
