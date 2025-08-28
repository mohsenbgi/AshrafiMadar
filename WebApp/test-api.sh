#!/bin/bash

echo "🏭 Testing Smart Factory Monitoring System API"
echo "=============================================="

# Test 1: Send normal sensor data
echo "📡 Test 1: Sending normal sensor data..."
curl -X POST "http://localhost:5000/api/sensor/raw" \
  -H "Content-Type: text/plain" \
  -d "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:892.00,Env_Humid:45.50,Light_Level:210,Gas_Methane:15,Gas_CO:61,Machine_Sound:512,Tank_Pressure:72,Main_Current:123,Engine_Vibe:25,Input_Voltage:230,Conveyor_Dist:150,Water_Leak:100,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:90"

echo -e "\n\n"

# Test 2: Send warning data
echo "⚠️ Test 2: Sending warning sensor data..."
curl -X POST "http://localhost:5000/api/sensor/raw" \
  -H "Content-Type: text/plain" \
  -d "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:950.00,Env_Humid:85.00,Light_Level:50,Gas_Methane:250,Gas_CO:85,Machine_Sound:650,Tank_Pressure:85,Main_Current:160,Engine_Vibe:60,Input_Voltage:200,Conveyor_Dist:0,Water_Leak:450,Flame_Status:0,Gate_Status:1,E_Stop_Button:0,Coolant_Valve:95"

echo -e "\n\n"

# Test 3: Send emergency data
echo "🚨 Test 3: Sending emergency sensor data..."
curl -X POST "http://localhost:5000/api/sensor/raw" \
  -H "Content-Type: text/plain" \
  -d "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:1050.00,Env_Humid:90.00,Light_Level:30,Gas_Methane:400,Gas_CO:120,Machine_Sound:700,Tank_Pressure:95,Main_Current:180,Engine_Vibe:80,Input_Voltage:180,Conveyor_Dist:0,Water_Leak:500,Flame_Status:1,Gate_Status:1,E_Stop_Button:1,Coolant_Valve:100"

echo -e "\n\n"

# Test 4: Get current data
echo "📊 Test 4: Getting current sensor data..."
curl -X GET "http://localhost:5000/api/sensor/current" \
  -H "Accept: application/json"

echo -e "\n\n"

# Test 5: Get sensor locations
echo "📍 Test 5: Getting sensor locations..."
curl -X GET "http://localhost:5000/api/sensor/locations" \
  -H "Accept: application/json"

echo -e "\n\n"

echo "✅ API tests completed!"
echo "🌐 Dashboard available at: http://localhost:5000"
echo "📡 API endpoints:"
echo "  - POST /api/sensor/raw (for raw sensor data)"
echo "  - POST /api/sensor/data (for JSON sensor data)"
echo "  - GET /api/sensor/current (get current data)"
echo "  - GET /api/sensor/locations (get sensor info)"
echo "  - POST /api/test/simulate (simulate normal data)"
echo "  - POST /api/test/simulate-warning (simulate warnings)"
echo "  - POST /api/test/simulate-emergency (simulate emergency)"