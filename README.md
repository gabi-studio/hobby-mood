# HobbyMood API 

HobbyMood is a **RESTful API** that helps users track their hobbies, experiences, and moods. 
The API allows users to log experiences, associate them with moods, and observe patterns in their activities and moods.

## Entities & Relationships  

- A **Hobby** can have many **Experiences**.  
- An **Experience** can be linked to many **Moods**.  
- **ExperienceMood** is a bridge table linking **Experiences** and **Moods** (Many-to-Many).

---

## **API Endpoints**  

### **Experience API**  
- **GET** `/api/Experience/List` - Get all experiences  
- **GET** `/api/Experience/Find/{id}` - Get an experience by ID  
- **POST** `/api/Experience/Add` - Add a new experience  
- **PUT** `/api/Experience/Update/{id}` - Update an experience  
- **DELETE** `/api/Experience/Delete/{id}` - Delete an experience  
- **PUT** `/api/Experience/UpdateExperienceImage/{id}` - Upload or update an experience image  

---


### **Hobby API**  
- **GET** `/api/Hobby/List` - Get all hobbies  
- **GET** `/api/Hobby/Find/{id}` - Get a hobby by ID  
- **GET** `/api/Hobby/Experiences/{hobbyId}` - Get all experiences linked to a hobby  
- **POST** `/api/Hobby/Add` - Add a new hobby  
- **PUT** `/api/Hobby/Update/{id}` - Update a hobby  
- **DELETE** `/api/Hobby/Delete/{id}` - Delete a hobby  
- **POST** `/api/Hobby/Link` - Link a hobby to an experience  
- **DELETE** `/api/Hobby/Unlink` - Unlink a hobby from an experience  

---

### **Mood API**  
- **GET** `/api/Mood/List` - Get all moods  
- **GET** `/api/Mood/Find/{id}` - Get a mood by ID  
- **GET** `/api/Mood/Experiences/{moodId}` - Get all experiences linked to a mood  
- **POST** `/api/Mood/Add` - Add a new mood  
- **PUT** `/api/Mood/Update/{id}` - Update a mood  
- **DELETE** `/api/Mood/Delete/{id}` - Delete a mood  

---

### **ExperienceMood API (Bridge Table)**  
- **GET** `/api/ExperienceMood/List` - Get all experience-mood associations  
- **GET** `/api/ExperienceMood/Find/{id}` - Get an experience-mood association by ID  
- **POST** `/api/ExperienceMood/Add` - Link a mood to an experience  
- **POST** `/api/ExperienceMood/Update/{id}` - Update an experience-mood association  
- **DELETE** `/api/ExperienceMood/Delete/{id}` - Remove a mood from an experience  
- **GET** `/api/ExperienceMood/ListForExperience/{experienceId}` - Get all moods linked to a specific experience  

---

## **Features**
- **Track your hobbies**: Create Experiences and categorize them by hobbies.
- **Log your experiences**: Add experiences with details such as **cost, location, duration, and date**.
- **Record your moods**: Attach moods to experiences and track emotional changes before and after the experience.



