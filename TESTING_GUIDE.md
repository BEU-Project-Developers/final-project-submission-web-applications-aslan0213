# Hotel Management System - Testing Guide

## System Overview
This is a complete Hotel Management System built with ASP.NET Core MVC, Entity Framework Core, and MSSQL database. The system supports user registration, authentication, room booking, payment processing, reviews, and admin management.

## Database Setup
- **Database**: MSSQL LocalDB
- **Status**: ✅ Successfully created and migrated
- **Seed Data**: ✅ Admin user and sample rooms created

## Test Scenarios

### 1. Admin Login Test
**Credentials:**
- Email: `admin@luxevoyage.com`
- Password: `admin123`
- Role: Admin

**Test Steps:**
1. Navigate to `/Account/Login`
2. Enter admin credentials
3. Verify redirect to admin dashboard
4. Check admin panel access in navigation

**Expected Results:**
- Successful login
- Access to admin dashboard
- Admin panel visible in navigation
- Dashboard shows statistics (bookings, users, rooms, revenue)

### 2. User Registration & Login Test
**Test Steps:**
1. Navigate to `/Account/Register`
2. Fill registration form with valid data
3. Submit form
4. Login with new credentials
5. Verify user dashboard access

**Expected Results:**
- Successful registration
- Account created in database
- Login works with new credentials
- User can access booking features

### 3. Room Browsing Test
**Available Room Types:**
- Deluxe Room (`/DeluxeRoom`)
- Executive Suite (`/SuiteRoom`)
- Family Room (`/FamilyRoom`)
- Premium Room (`/PremiumRoom`)
- Standard Room (`/StandardRoom`)

**Test Steps:**
1. Visit each room type page
2. Check room details display
3. Verify booking buttons work
4. Test responsive design

**Expected Results:**
- All room pages load correctly
- Room details, images, and features display
- Booking integration works
- Mobile-friendly design

### 4. Room Search & Booking Test
**Test Steps:**
1. Navigate to `/Booking/Search`
2. Enter search criteria:
   - Check-in date: Tomorrow
   - Check-out date: Day after tomorrow
   - Number of guests: 2
3. Search for available rooms
4. Select a room and click "Book Now"
5. Fill booking form
6. Proceed to payment

**Expected Results:**
- Search shows available rooms
- Booking form populates correctly
- Date validation works
- Guest number limits enforced

### 5. Payment Processing Test
**Test Steps:**
1. Complete booking process to payment page
2. Fill payment form:
   - Card Number: 4111111111111111 (test)
   - Cardholder Name: Test User
   - Expiry: 12/25
   - CVV: 123
3. Submit payment
4. Verify booking confirmation

**Expected Results:**
- Payment form validates correctly
- Booking created in database
- Confirmation page displays
- Email confirmation (if configured)

### 6. Review System Test
**Test Steps:**
1. Complete a booking
2. Navigate to "My Bookings"
3. Click "Write Review" for completed booking
4. Submit review with rating and comment
5. Verify review appears

**Expected Results:**
- Review form displays correctly
- Star rating system works
- Review saved to database
- Review appears on room page

### 7. Admin Management Test
**Test Steps:**
1. Login as admin
2. Navigate to admin dashboard
3. Test features:
   - View booking statistics
   - Manage bookings
   - Manage rooms
   - View recent activity

**Expected Results:**
- Dashboard loads with real data
- Statistics display correctly
- Admin can modify bookings
- Room management works

## API Endpoints Test

### Authentication Endpoints
- `POST /Account/Register` - User registration
- `POST /Account/Login` - User login
- `POST /Account/Logout` - User logout

### Booking Endpoints
- `GET /Booking/Search` - Room search
- `POST /Booking/Book` - Create booking
- `GET /Booking/MyBookings` - User bookings
- `POST /Booking/Payment` - Process payment

### Admin Endpoints (Requires Admin Role)
- `GET /Admin/Dashboard` - Admin dashboard
- `GET /Admin/ManageBookings` - Booking management
- `GET /Admin/ManageRooms` - Room management

### Room Type Endpoints
- `GET /DeluxeRoom` - Deluxe room showcase
- `GET /SuiteRoom` - Suite room showcase
- `GET /FamilyRoom` - Family room showcase
- `GET /PremiumRoom` - Premium room showcase
- `GET /StandardRoom` - Standard room showcase

## Database Schema

### Tables Created
1. **Users** - User accounts and authentication
2. **Hotels** - Hotel information
3. **Rooms** - Room inventory and details
4. **Bookings** - Reservation records
5. **Payments** - Payment transactions
6. **Reviews** - User reviews and ratings

### Seed Data
- 1 Hotel: LuxeVoyage Hotel
- 1 Admin User: admin@luxevoyage.com
- 5 Rooms: Various types (Deluxe, Suite, Standard, Premium, Family)

## Known Issues Fixed
1. ✅ ViewModel property mismatches resolved
2. ✅ Database relationship conflicts fixed
3. ✅ Missing navigation properties added
4. ✅ Build errors resolved
5. ✅ ViewData compatibility ensured

## Security Features
- Password authentication
- Role-based authorization (Admin/User)
- CSRF protection
- SQL injection prevention (EF Core)
- Input validation and sanitization

## Performance Features
- Entity Framework Core optimizations
- Lazy loading for related data
- Pagination for large datasets
- Responsive design for mobile devices

## Testing Checklist
- [ ] Admin login works
- [ ] User registration works
- [ ] Room search functionality
- [ ] Booking creation process
- [ ] Payment processing
- [ ] Review submission
- [ ] Admin dashboard access
- [ ] Room management
- [ ] Booking management
- [ ] Mobile responsiveness
- [ ] Error handling
- [ ] Input validation

## URLs for Testing
- **Home**: http://localhost:5002/
- **Login**: http://localhost:5002/Account/Login
- **Register**: http://localhost:5002/Account/Register
- **Search**: http://localhost:5002/Booking/Search
- **Admin**: http://localhost:5002/Admin/Dashboard
- **Room Types**: http://localhost:5002/[RoomType] (DeluxeRoom, SuiteRoom, etc.)

## Support
For issues or questions, check the code comments and controller implementations for detailed functionality explanations.
