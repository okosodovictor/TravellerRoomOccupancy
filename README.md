# README section

## Architecture
This solution uses Onion architecture to separate domain, application, and infrastructure concerns.

## Testing
The solution includes unit tests and integration tests using Testcontainers.

## Not implemented
- Security: JWT authentication/authorization.
- Observability: OpenTelemetry tracing/metrics/logging.
- Deployment: Kubernetes manifests (YAML) for production deployment.

## Quick demo

### Prerequisites
- PostgreSQL running and `ConnectionStrings:OccupancyDb` configured (or use your dev setup).
- Run the WebApi in Development, or use Docker Compose.
- Swagger should be available at `http://localhost:8080/swagger/index.html` (or your configured port).

To run with Docker Compose:

```sh
docker compose up
```

### Seeded demo data (Development only)
When running in Development, the API auto-applies migrations and seeds demo data (if the DB is empty).

- Seeded HotelId: `11111111-1111-1111-1111-111111111111`
- Seeded rooms:
  - Room `0101` (BedCount = 2)
  - Room `0102` (BedCount = 1)
- Seeded group: GroupId `A12345`
- Seeded travellers:
  - John Doe, DOB 1990-01-01 (initially assigned to room `0101` for "today")
  - Jane Doe, DOB 1991-02-02 (exists, not assigned initially)

"Today" is calculated using UTC date on the server.

### Swagger demo steps

#### List occupied rooms today
Open `GET /api/occupancy/today`, set query:

```
hotelId = 11111111-1111-1111-1111-111111111111
```

Execute -> should show room `0101` with `OccupiedBeds = 1`.

#### Check occupancy for a specific date
Open `GET /api/occupancy/date/{date}`.

Example:

```
date = 2026-01-14 (or today's UTC date)
hotelId = 11111111-1111-1111-1111-111111111111
```

#### Get individual room occupancy
Open `GET /api/occupancy/rooms/{roomCode}`.

Example:

```
roomCode = 0101
hotelId = 11111111-1111-1111-1111-111111111111
date = <today>
```

Execute -> should list John Doe in travellers.

#### Assign Jane to a room
Open `POST /api/assignments/assign`.

Body:

```json
{
  "hotelId": "11111111-1111-1111-1111-111111111111",
  "date": "2026-01-14",
  "traveller": {
    "groupId": "A12345",
    "surname": "Doe",
    "firstName": "Jane",
    "dateOfBirth": "1991-02-02"
  },
  "roomCode": "0101"
}
```

Expected: 204 No Content.

#### Move John from 0101 to 0102
Open `POST /api/assignments/move`.

Body:

```json
{
  "hotelId": "11111111-1111-1111-1111-111111111111",
  "date": "2026-01-14",
  "traveller": {
    "groupId": "A12345",
    "surname": "Doe",
    "firstName": "John",
    "dateOfBirth": "1990-01-01"
  },
  "fromRoomCode": "0101",
  "toRoomCode": "0102"
}
```

Expected: 204 No Content.

#### Demonstrate capacity constraint (room full)
Since room `0102` has BedCount = 1, after moving John to `0102`, try assigning Jane to `0102`:

```json
{
  "hotelId": "11111111-1111-1111-1111-111111111111",
  "date": "2026-01-14",
  "traveller": {
    "groupId": "A12345",
    "surname": "Doe",
    "firstName": "Jane",
    "dateOfBirth": "1991-02-02"
  },
  "roomCode": "0102"
}
```

Expected: 409 Conflict with code `room_full`.

## Curl demo (copy/paste)

Set variables:

```sh
export BASE_URL="http://localhost:8080"
export HOTEL_ID="11111111-1111-1111-1111-111111111111"
export DATE="2026-01-14"
```

Occupied rooms today:

```sh
curl "$BASE_URL/occupancy/today?hotelId=$HOTEL_ID"
```

Occupied rooms for date:

```sh
curl "$BASE_URL/occupancy/date/$DATE?hotelId=$HOTEL_ID"
```

Room occupancy:

```sh
curl "$BASE_URL/occupancy/rooms/0101?hotelId=$HOTEL_ID&date=$DATE"
```

Assign Jane to 0101:

```sh
curl -X 'POST' \
  "$BASE_URL/assignments/assign" \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "hotelId": "'"$HOTEL_ID"'",
  "date": "'"$DATE"'",
  "traveller": {
    "groupId": "A12345",
    "surname": "Doe",
    "firstName": "Jane",
    "dateOfBirth": "1991-02-02"
  },
  "roomCode": "0101"
}'
```

Move John 0101 -> 0102:

```sh
curl -X 'POST' \
  "$BASE_URL/assignments/move" \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "hotelId": "'"$HOTEL_ID"'",
  "date": "'"$DATE"'",
  "traveller": {
    "groupId": "A12345",
    "surname": "Doe",
    "firstName": "John",
    "dateOfBirth": "1990-01-01"
  },
  "fromRoomCode": "0101",
  "toRoomCode": "0102"
}'
```

## Documented assumptions and uncertainties

## Acceptance criteria vs APIs

1) "Request all occupied rooms from today"  
API: `GET /api/occupancy/today?hotelId={hotelId}`  
Returns: list of occupied rooms for today (server-side date), including RoomCode, BedCount, OccupiedBeds.  
Notes: Uses UTC date in code (`DateTime.UtcNow.Date`).

2) "Request all rooms booked by a travel group"  
API: `GET /api/occupancy/groups/{groupId}/rooms?hotelId={hotelId}&date={YYYY-MM-DD}`  
Returns: rooms occupied by that group for the specified day.  
Ambiguity: "rooms booked by a travel group" could mean a single day (implemented) or whole stay (needs stay length/checkout).

3) "Request individual rooms"  
API: `GET /api/occupancy/rooms/{roomCode}?hotelId={hotelId}&date={YYYY-MM-DD}`  
Returns: room capacity plus travellers assigned to that room on that date.

4) "Move travellers from one room to another"  
API: `POST /api/assignments/move`  
Behavior: validates traveller/group membership and source room, prevents over-occupancy, returns 204 on success, 409 on conflicts, 404 on not found.

5) "Interface to assign persons to rooms... report changes"  
API: `POST /api/assignments/assign`  
Note: Added to enable initial assignments so move/query endpoints have meaningful data.

### Assumptions (explicit)

**Occupancy is modeled per day (DateOnly)**  
The requirement says "daily room occupancy". I chose a per-date model so the hotel can ask "who sleeps in which room on this date".

**No checkout date / stay length is provided**  
Travel groups only have an arrival date and traveller count. I interpreted "rooms booked by a travel group" as rooms assigned on a specific date (the API requires a date).

**Traveller identity is (Surname, FirstName, DateOfBirth) within a travel group**  
This matches the requirement. I treat this combination as sufficiently unique within a group; if not, I'd add a system-generated TravellerId exposed to the hotel.

**Hotel context is passed via hotelId**  
In production I'd derive hotel identity from authentication (API key/JWT) instead of a query parameter. For this task I kept it explicit.

**"Today" uses UTC date**  
To avoid server-local timezone surprises, I used UTC. If the business expects local hotel time, I'd introduce IClock + hotel timezone config.

### Uncertainties (called out)

**Does "rooms booked by a travel group" mean "for today" or "for the whole stay"?**  
Current implementation is per date. If "whole stay" is required, I would need either:

- a checkout date / nights count, or
- assignment generation rules across multiple dates.

**Concurrency guarantee level**  
I implemented writes with a serializable transaction and room row lock to prevent over-occupancy under concurrent moves/assignments. If extremely high throughput is expected, I'd refine the locking strategy (e.g., advisory locks or more targeted row locks).

**RoomCode and GroupId normalization**  
I normalize GroupId casing (upper). RoomCode is treated as exact string representing a numeric 4-digit code; any formatting differences (e.g., "101" vs "0101") should be clarified, and current domain parsing should enforce 4 digits.
