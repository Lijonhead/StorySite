# 🖋️ StorySite

**StorySite** is the frontend web application for the StoryPrompt platform — a community-driven space where users can respond to daily writing prompts with creative stories. This ASP.NET Core MVC app connects to the backend [StoryPromptAPI](https://github.com/Lijonhead/StoryPromptAPI) to handle user authentication, story submissions, and content interactions like voting and profile management.

---

## 🧭 Overview

- Shows daily writing prompts retrieved from the API
- Allows users to register, log in, and manage their profile
- Users can write and submit stories
- Voting system to upvote/downvote stories and prompts
- Authenticated sessions are managed with cookie-based authentication
- Admins have special access to prompt management (via policy)

---

## 🛠️ Tech Stack

- **ASP.NET Core MVC (.NET 8)**
- **Razor Pages** and **Controllers**
- **Cookie Authentication** with custom login/logout paths
- **HttpClient** for API integration with [StoryPromptAPI](https://github.com/Lijonhead/StoryPromptAPI)

---

## 🔐 Authentication Setup

- Uses **Cookie-based authentication**
- Secure cookie settings with 1-hour expiration and sliding expiration
- Policies for admin-only features using role-based access

---

## 🌐 External API

This frontend communicates with the backend API that used to be hosted on Azure but can be changed to work locally.
