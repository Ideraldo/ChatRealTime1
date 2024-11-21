# Personalized Chatbot with LLM Integration

This repository hosts a **Personalized Chatbot** built using a local Large Language Model (LLM) powered by [Ollama](https://ollama.ai/), with a backend in `.NET` and a frontend in `React`. The project leverages `SignalR` for real-time communication between the backend and frontend. Additionally, the system is designed to support **RAG (Retrieval Augmented Generation)** techniques to enhance the chatbot's contextual understanding and response accuracy in future iterations.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Architecture Overview](#architecture-overview)
- [Future Enhancements](#future-enhancements)

![image](https://github.com/user-attachments/assets/1f6cbf7b-f7e0-4ecf-af5b-edc8419cc043)


---

## Features

- **Real-Time Communication**: Seamless user experience enabled by SignalR for real-time messaging.
- **Local LLM Integration**: Utilizes Ollama to ensure privacy and control over language model behavior.
- **Scalable Backend**: Built with `.NET`, following best practices like SOLID, Clean Architecture, and dependency injection.
- **Dynamic Frontend**: Built in `React` with a responsive design.
- **Future RAG Support**: Framework ready for integrating external document retrieval to enhance context-aware responses.

---

## Technologies Used

### Backend
- **.NET 6+**: Backend framework.
- **SignalR**: For bi-directional communication between client and server.
- **Ollama**: Local LLM to handle natural language processing.
- **Entity Framework Core** (optional): For data persistence (if needed).
- **Dependency Injection**: For managing services and enhancing testability.

### Frontend
- **React 18+**: Frontend framework.
- **SignalR Client**: To establish communication with the backend.

### Development and Deployment
- **Docker**: For containerization.
- **Azure** (optional): For hosting if not running locally.
- **RAG Framework**: Integration ready (e.g., with Azure Cognitive Search or ElasticSearch).

---

## Architecture Overview

### 1. **Frontend**  
   - The React application handles user interactions and displays real-time conversations.  
   - SignalR is used to maintain a persistent WebSocket connection to the backend.  

### 2. **Backend**  
   - The `.NET` backend handles requests, routes data between the frontend and Ollama, and manages future RAG queries.  
   - SignalR Hub serves as the central point for real-time communication.  
   - Ollama processes incoming user inputs and generates responses.  

### 3. **RAG Integration** 
   - A `Retriever` module will query external document sources to provide context-aware responses.  
   - Combined with Ollama, this approach will use retrieved information to enhance language model responses.

---

# Future Enhancements
RAG Integration:

Implement a document retriever using Azure Cognitive Search, ElasticSearch, or local databases.
Enhance responses by combining retrieved data with Ollama-generated outputs.
Authentication:

Add user authentication and authorization to manage personalized conversations.
Persistent Chat Logs:

Store user messages and responses for better continuity.
Advanced Frontend UI:

Improve the chatbot interface with animations and accessibility features.
Scalability:

Introduce microservices for the backend and container orchestration via Kubernetes.
