# RaceDay API Endpoint Plan

Roles: **Organiser** (creates/manages events, categories, results), **Participant** (browses events, enrols, views own results). "Owner" means the Organiser who created that specific Event/Category.

## Authentication

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/auth/register | Registers a new user as Organiser or Participant | None (public) | `{ fullName, email, password, role }` | 201 Created – new user object (no password); 409 Conflict – email already registered |
| POST | /api/auth/login | Authenticates a user and returns an access token | None (public) | `{ email, password }` | 200 OK – `{ token, user }`; 401 Unauthorized – invalid credentials |

## User Profile

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | /api/users/me | Returns the logged-in user's profile | Any (logged in) | None | 200 OK – user profile; 401 Unauthorized |
| PUT | /api/users/me | Updates the logged-in user's profile | Any (logged in) | `{ fullName, email }` | 200 OK – updated profile; 400 Bad Request |

## Events

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | /api/events | Lists all events | None (public) | None | 200 OK – array of events |
| GET | /api/events/{id} | Gets details of a single event | None (public) | None | 200 OK – event; 404 Not Found |
| POST | /api/events | Creates a new event | Organiser | `{ name, description, eventDate, location }` | 201 Created – event; 403 Forbidden |
| PUT | /api/events/{id} | Updates an event | Organiser (owner) | `{ name, description, eventDate, location }` | 200 OK – updated event; 403 Forbidden; 404 Not Found |
| DELETE | /api/events/{id} | Deletes an event | Organiser (owner) | None | 200 OK; 403 Forbidden; 404 Not Found |

## Categories

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | /api/events/{id}/categories | Lists all categories for an event | None (public) | None | 200 OK – array of categories |
| POST | /api/events/{id}/categories | Adds a category to an event | Organiser (owner) | `{ name, distanceKm, maxParticipants }` | 201 Created – category; 403 Forbidden; 404 Not Found |
| PUT | /api/categories/{id} | Updates a category | Organiser (owner) | `{ name, distanceKm, maxParticipants }` | 200 OK; 403 Forbidden; 404 Not Found |
| DELETE | /api/categories/{id} | Deletes a category | Organiser (owner) | None | 200 OK; 403 Forbidden; 404 Not Found |

## Event Enrolments

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/categories/{id}/enrol | Enrols the logged-in participant into a category | Participant | `{ }` (bib number auto-assigned) | 201 Created – enrolment; 409 Conflict – already enrolled or category full |
| GET | /api/users/me/enrolments | Lists the logged-in participant's own enrolments | Participant | None | 200 OK – array of enrolments |
| GET | /api/categories/{id}/enrolments | Lists all enrolments for a category | Organiser (owner) | None | 200 OK – array of enrolments; 403 Forbidden |
| DELETE | /api/enrolments/{id} | Cancels an enrolment | Participant (owner) or Organiser (owner) | None | 200 OK; 403 Forbidden; 404 Not Found |

## Results

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/enrolments/{id}/result | Records a result for an enrolment | Organiser (owner) | `{ finishTime, position }` | 201 Created – result; 403 Forbidden; 404 Not Found |
| GET | /api/categories/{id}/results | Gets the results/leaderboard for a category | None (public) | None | 200 OK – array of results |
| GET | /api/results/{id} | Gets a single result | None (public) | None | 200 OK – result; 404 Not Found |
| PUT | /api/results/{id} | Updates a result | Organiser (owner) | `{ finishTime, position }` | 200 OK – updated result; 403 Forbidden; 404 Not Found |
