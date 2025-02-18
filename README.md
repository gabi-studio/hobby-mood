# HobbyMood API 
HobbyMood is a **RESTful API** that helps users track their hobbies, experiences, and moods. 
The API allows users to log experiences, associate them with moods, and observe patterns in their activities and moods.

## Entities & Relationships  

- A **Hobby** can have many **Experiences**.  
- An **Experience** can be linked to many **Moods**.  
- **ExperienceMood** is a bridge table linking **Experiences** and **Moods** (Many-to-Many).


## API Endpoints  

### Hobby API  
- **GET** `/api/Hobby/List` - Get all hobbies  
- **GET** `/api/Hobby/Find/{id}` - Get a hobby by ID  
- **POST** `/api/Hobby/Add` - Add a hobby  
- **PUT** `/api/Hobby/Update/{id}` - Update a hobby  
- **DELETE** `/api/Hobby/Delete/{id}` - Delete a hobby  

### Experience API  
- **GET** `/api/Experience/List` - Get all experiences  
- **GET** `/api/Experience/Find/{id}` - Get an experience by ID  
- **POST** `/api/Experience/Add` - Add an experience  
- **PUT** `/api/Experience/Update/{id}` - Update an experience  
- **DELETE** `/api/Experience/Delete/{id}` - Delete an experience  

### Mood API  
- **GET** `/api/Mood/List` - Get all moods  
- **GET** `/api/Mood/Find/{id}` - Get a mood by ID  
- **POST** `/api/Mood/Add` - Add a mood  
- **PUT** `/api/Mood/Update/{id}` - Update a mood  
- **DELETE** `/api/Mood/Delete/{id}` - Delete a mood  

### ExperienceMood API (Bridge)  
- **GET** `/api/ExperienceMood/List` - Get all experience-mood associations  
- **GET** `/api/ExperienceMood/Find/{id}` - Get an experience-mood association by ID  
- **POST** `/api/ExperienceMood/Add` - Add an experience-mood association  
- **PUT** `/api/ExperienceMood/Update/{id}` - Update an experience-mood association  
- **DELETE** `/api/ExperienceMood/Delete/{id}` - Delete an experience-mood association  




