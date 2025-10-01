# 📱 FREE1X2 MOBILE INTEGRATION GUIDE
## Complete Developer Documentation for iOS & Android Apps

### 🎯 Overview
This guide provides everything needed to integrate Free1X2 Web API into mobile applications. The API maintains the same mathematical accuracy as the original desktop application while providing modern REST endpoints.

---

## 🚀 Quick Start

### API Base Information
- **Base URL**: `http://localhost:5262` (Development)
- **Production URL**: `https://your-domain.com` (To be configured)
- **API Version**: v1
- **Documentation**: `{BASE_URL}/swagger` (Interactive API explorer)

### Authentication
Currently no authentication required (can be added later for production)

---

## 📋 Core API Endpoints

### 1. 🏆 Teams Management

#### Get All Teams
```http
GET /api/Teams
```
**Response Example:**
```json
[
  {
    "id": 1,
    "name": "Real Madrid",
    "shortName": "RMA",
    "category": "Primera División",
    "isActive": true
  },
  {
    "id": 2, 
    "name": "FC Barcelona",
    "shortName": "FCB",
    "category": "Primera División",
    "isActive": true
  }
]
```

#### Get Team by ID
```http
GET /api/Teams/{id}
```

#### Search Teams
```http
GET /api/Teams/search?query={searchTerm}
```

### 2. 🧮 Analysis Engine (Core Football Pools Logic)

#### Calculate Column Probabilities
```http
POST /api/Analysis/column-probabilities
Content-Type: application/json

{
  "matches": [
    {
      "homeTeamId": 1,
      "awayTeamId": 2,
      "homeTeamName": "Real Madrid",
      "awayTeamName": "FC Barcelona",
      "date": "2025-10-15T20:00:00Z"
    }
  ],
  "filterOptions": {
    "enableConsecutiveFilter": true,
    "enableEconomicFilter": true,
    "maxCost": 100.0
  }
}
```

**Response Example:**
```json
{
  "analysisId": "uuid-here",
  "totalColumns": 1024,
  "filteredColumns": 256,
  "executionTimeMs": 45,
  "probabilities": [
    {
      "column": "1X2X1X...",
      "probability": 0.125,
      "cost": 2.50,
      "expectedValue": 15.75
    }
  ],
  "statistics": {
    "averageProbability": 0.00390625,
    "totalCost": 640.0,
    "recommendedColumns": 10
  }
}
```

#### Get Analysis History
```http
GET /api/Analysis/history?page=1&pageSize=20
```

### 3. 🔍 Filter System

#### Get Available Filters
```http
GET /api/Filters
```

**Response Example:**
```json
[
  {
    "id": "consecutive",
    "name": "Consecutive Filter",
    "description": "Filters based on consecutive outcomes",
    "parameters": [
      {
        "name": "maxConsecutive",
        "type": "integer",
        "defaultValue": 3,
        "min": 1,
        "max": 10
      }
    ]
  },
  {
    "id": "economic", 
    "name": "Economic Filter",
    "description": "Cost-based optimization filter",
    "parameters": [
      {
        "name": "maxCost",
        "type": "decimal",
        "defaultValue": 100.0,
        "min": 1.0,
        "max": 1000.0
      }
    ]
  }
]
```

#### Apply Filters
```http
POST /api/Filters/apply
Content-Type: application/json

{
  "columns": ["1X2", "X12", "2X1"],
  "filters": [
    {
      "type": "consecutive",
      "parameters": {
        "maxConsecutive": 3
      }
    },
    {
      "type": "economic",
      "parameters": {
        "maxCost": 50.0
      }
    }
  ]
}
```

### 4. ⚙️ Configuration

#### Get API Information
```http
GET /api/Configuration/info
```

**Response Example:**
```json
{
  "apiName": "Free1X2 Web API",
  "version": "0.77.1",
  "description": "Football pools analysis backend",
  "environment": "Development",
  "timestamp": "2025-09-30T21:14:47.123Z",
  "configuration": {
    "language": "es-ES",
    "currency": "EUR",
    "timezone": "Europe/Madrid",
    "dateFormat": "dd/MM/yyyy"
  },
  "features": [
    "ColumnProbabilities",
    "FilterSystem", 
    "TeamManagement",
    "AnalysisHistory"
  ]
}
```

#### Get System Settings
```http
GET /api/Configuration/settings
```

---

## 📱 Platform-Specific Integration

### iOS Integration (Swift/SwiftUI)

#### 1. HTTP Client Setup
```swift
import Foundation

class Free1X2APIClient: ObservableObject {
    private let baseURL = "http://localhost:5262"
    private let session = URLSession.shared
    
    func getTeams() async throws -> [Team] {
        let url = URL(string: "\(baseURL)/api/Teams")!
        let (data, _) = try await session.data(from: url)
        return try JSONDecoder().decode([Team].self, from: data)
    }
    
    func calculateProbabilities(matches: [Match], filters: FilterOptions) async throws -> AnalysisResult {
        let url = URL(string: "\(baseURL)/api/Analysis/column-probabilities")!
        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        
        let requestBody = ColumnProbabilityRequest(matches: matches, filterOptions: filters)
        request.httpBody = try JSONEncoder().encode(requestBody)
        
        let (data, _) = try await session.data(for: request)
        return try JSONDecoder().decode(AnalysisResult.self, from: data)
    }
}
```

#### 2. Data Models
```swift
struct Team: Codable, Identifiable {
    let id: Int
    let name: String
    let shortName: String
    let category: String
    let isActive: Bool
}

struct Match: Codable {
    let homeTeamId: Int
    let awayTeamId: Int
    let homeTeamName: String
    let awayTeamName: String
    let date: Date
}

struct AnalysisResult: Codable {
    let analysisId: String
    let totalColumns: Int
    let filteredColumns: Int
    let executionTimeMs: Int
    let probabilities: [ColumnProbability]
    let statistics: AnalysisStatistics
}
```

#### 3. SwiftUI Views
```swift
struct TeamsListView: View {
    @StateObject private var apiClient = Free1X2APIClient()
    @State private var teams: [Team] = []
    
    var body: some View {
        NavigationView {
            List(teams) { team in
                TeamRowView(team: team)
            }
            .navigationTitle("Teams")
            .task {
                do {
                    teams = try await apiClient.getTeams()
                } catch {
                    print("Error loading teams: \(error)")
                }
            }
        }
    }
}
```

### Android Integration (Kotlin/Jetpack Compose)

#### 1. Retrofit Setup
```kotlin
// build.gradle.kts (Module: app)
dependencies {
    implementation("com.squareup.retrofit2:retrofit:2.9.0")
    implementation("com.squareup.retrofit2:converter-gson:2.9.0")
    implementation("com.squareup.okhttp3:logging-interceptor:4.11.0")
}
```

#### 2. API Service Interface
```kotlin
import retrofit2.Response
import retrofit2.http.*

interface Free1X2ApiService {
    @GET("api/Teams")
    suspend fun getTeams(): Response<List<Team>>
    
    @GET("api/Teams/{id}")
    suspend fun getTeam(@Path("id") teamId: Int): Response<Team>
    
    @POST("api/Analysis/column-probabilities")
    suspend fun calculateProbabilities(
        @Body request: ColumnProbabilityRequest
    ): Response<AnalysisResult>
    
    @GET("api/Filters")
    suspend fun getAvailableFilters(): Response<List<FilterDefinition>>
}
```

#### 3. Data Classes
```kotlin
data class Team(
    val id: Int,
    val name: String,
    val shortName: String,
    val category: String,
    val isActive: Boolean
)

data class Match(
    val homeTeamId: Int,
    val awayTeamId: Int,
    val homeTeamName: String,
    val awayTeamName: String,
    val date: String
)

data class AnalysisResult(
    val analysisId: String,
    val totalColumns: Int,
    val filteredColumns: Int,
    val executionTimeMs: Int,
    val probabilities: List<ColumnProbability>,
    val statistics: AnalysisStatistics
)
```

#### 4. Repository Pattern
```kotlin
class Free1X2Repository @Inject constructor(
    private val apiService: Free1X2ApiService
) {
    suspend fun getTeams(): Result<List<Team>> {
        return try {
            val response = apiService.getTeams()
            if (response.isSuccessful) {
                Result.success(response.body() ?: emptyList())
            } else {
                Result.failure(Exception("API Error: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun calculateProbabilities(
        matches: List<Match>,
        filters: FilterOptions
    ): Result<AnalysisResult> {
        return try {
            val request = ColumnProbabilityRequest(matches, filters)
            val response = apiService.calculateProbabilities(request)
            if (response.isSuccessful) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Analysis failed: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
```

#### 5. Jetpack Compose UI
```kotlin
@Composable
fun TeamsScreen(
    viewModel: TeamsViewModel = hiltViewModel()
) {
    val teams by viewModel.teams.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    
    LaunchedEffect(Unit) {
        viewModel.loadTeams()
    }
    
    Column {
        if (isLoading) {
            Box(
                modifier = Modifier.fillMaxSize(),
                contentAlignment = Alignment.Center
            ) {
                CircularProgressIndicator()
            }
        } else {
            LazyColumn {
                items(teams) { team ->
                    TeamItem(
                        team = team,
                        onClick = { /* Navigate to team details */ }
                    )
                }
            }
        }
    }
}
```

---

## 🔧 Advanced Integration Features

### Real-time Updates (Future Enhancement)
```javascript
// WebSocket connection for live updates
const socket = new WebSocket('ws://localhost:5262/analysis-hub');

socket.onmessage = (event) => {
    const update = JSON.parse(event.data);
    if (update.type === 'ANALYSIS_COMPLETE') {
        updateUI(update.result);
    }
};
```

### Offline Support Strategy
```swift
// iOS Core Data integration
class OfflineDataManager {
    func cacheTeams(_ teams: [Team]) {
        // Save to Core Data for offline access
    }
    
    func getCachedTeams() -> [Team] {
        // Retrieve from Core Data when offline
    }
    
    func syncWhenOnline() async {
        // Sync cached data when connection restored
    }
}
```

### Performance Optimization
```kotlin
// Android caching with Room
@Entity(tableName = "teams")
data class TeamEntity(
    @PrimaryKey val id: Int,
    val name: String,
    val shortName: String,
    val category: String,
    val isActive: Boolean,
    val lastUpdated: Long = System.currentTimeMillis()
)

@Dao
interface TeamDao {
    @Query("SELECT * FROM teams WHERE isActive = 1")
    suspend fun getActiveTeams(): List<TeamEntity>
    
    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertTeams(teams: List<TeamEntity>)
}
```

---

## 🚨 Error Handling Best Practices

### Common HTTP Status Codes
- `200 OK` - Success
- `400 Bad Request` - Invalid request data
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### Error Response Format
```json
{
  "error": {
    "code": "INVALID_MATCH_DATA",
    "message": "Match data validation failed",
    "details": [
      "homeTeamId is required",
      "awayTeamId must be different from homeTeamId"
    ],
    "timestamp": "2025-09-30T21:30:00Z"
  }
}
```

### Error Handling Implementation
```swift
// iOS Error Handling
enum APIError: LocalizedError {
    case invalidResponse
    case networkError(Error)
    case serverError(Int)
    case decodingError(Error)
    
    var errorDescription: String? {
        switch self {
        case .invalidResponse:
            return "Invalid response from server"
        case .networkError(let error):
            return "Network error: \(error.localizedDescription)"
        case .serverError(let code):
            return "Server error with code: \(code)"
        case .decodingError(let error):
            return "Data parsing error: \(error.localizedDescription)"
        }
    }
}
```

---

## 🧪 Testing Your Integration

### API Testing Checklist
- [ ] Teams loading and display
- [ ] Match creation and validation
- [ ] Probability calculations
- [ ] Filter application
- [ ] Error scenarios handling
- [ ] Offline behavior
- [ ] Performance under load

### Sample Test Data
```json
{
  "testMatches": [
    {
      "homeTeamId": 1,
      "awayTeamId": 2,
      "homeTeamName": "Real Madrid",
      "awayTeamName": "FC Barcelona",
      "date": "2025-10-15T20:00:00Z"
    }
  ],
  "expectedProbabilities": 1024,
  "expectedFilteredResults": "varies by filter"
}
```

---

## 🚀 Deployment Considerations

### Production API Setup
1. **HTTPS Configuration**: Ensure SSL/TLS certificates
2. **Domain Configuration**: Update base URLs in mobile apps
3. **Rate Limiting**: Implement API rate limits
4. **Authentication**: Add JWT or OAuth if required
5. **Monitoring**: Set up application monitoring
6. **Backup Strategy**: Ensure data backup procedures

### Mobile App Store Requirements
- **iOS**: Update Info.plist with network permissions
- **Android**: Add INTERNET permission in AndroidManifest.xml
- **Both**: Handle network security configurations

---

## 📞 Support & Resources

### API Documentation
- **Interactive Docs**: `{BASE_URL}/swagger`
- **OpenAPI Spec**: `{BASE_URL}/swagger/v1/swagger.json`

### Sample Projects
- iOS Sample App: [Coming Soon]
- Android Sample App: [Coming Soon]
- React Native Example: [Coming Soon]

### Contact Information
- **Technical Support**: [Your Contact Info]
- **API Issues**: Check server logs and API response codes
- **Feature Requests**: Submit through your preferred channel

---

*Last Updated: September 30, 2025*
*API Version: 0.77.1*