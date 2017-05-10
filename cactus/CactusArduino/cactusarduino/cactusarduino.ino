#include <Adafruit_GFX.h>
#include "Adafruit_LEDBackpack.h"
#include "TimerOne.h"
#include <SD.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <SPI.h>

char ssid[] = "TP-LINK_A8FCC8";     // the name of your network
int status = WL_IDLE_STATUS;     // the Wifi radio's status
char pass[] = "59053491";    // your network key
unsigned int localPort = 2390;      // local port to listen on
char packetBuffer[255]; //buffer to hold incoming packet
char  ReplyBuffer[] = "xxxx";       // a string to send back
WiFiUDP Udp;

Adafruit_8x8matrix matrix[5];
const uint8_t MATRIX_EYEL = 0;
const uint8_t MATRIX_EYER = 1;
const uint8_t MATRIX_MOUTHL = 2;
const uint8_t MATRIX_MOUTHM = 3;
const uint8_t MATRIX_MOUTHR = 4;

String filename[5];

int remainingTime[5];

uint8_t frame[5];

bool updateMatrix[5];

File readingFile;

void setup() {
  //Init
  for(int i = 0; i < 5; i ++) {
    filename[i] = "";
    remainingTime[i] = 0;
    frame[i] = 0;
    updateMatrix[i] = false;
  }
  
  Serial.begin(9600);
  if(!SD.begin(4)) {
    Serial.println("SD initialization failed!");
    return;
  }
  Serial.println("Starting...");
  matrix[MATRIX_MOUTHL].begin(0x70);
  matrix[MATRIX_MOUTHR].begin(0x71);
  matrix[MATRIX_MOUTHM].begin(0x72);
  matrix[MATRIX_EYER].begin(0x74);
  matrix[MATRIX_EYEL].begin(0x76);

  //playAnimation("test1_", MATRIX_MOUTHM);
  //playAnimation("test1_", MATRIX_MOUTHL);
  /*Run timeBase every 50 ms*/
  //Timer1.initialize(0.05 * 1000000);
  //Timer1.initialize(5 * 1000000);
  //Timer1.attachInterrupt(timeBase);

  //Init Wifi
   while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to WPA SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network:    
    status = WiFi.begin(ssid, pass);

    // wait 10 seconds for connection:
    delay(10000);
  }
  // you're connected now, so print out the data:
  Serial.print("You're connected to the network");
  Udp.begin(localPort);
}

void playAnimation(String name, uint8_t m) {
  filename[m] = name;
  remainingTime[m] = 0;
  frame[m] = 0;
}

void timeBase() {/*
  Serial.println("timebase");
  for(int m = 0; m < 5; m ++) {
    if(!updateMatrix[m]) {
      if(remainingTime[m] == 0) {
        updateMatrix[m] = true;
      }
      remainingTime[m] --;
    }
  }*/
}

void loop() {/*
  for(int m = 0; m < 5; m ++) {
    if(updateMatrix[m]) {
      remainingTime[m] = loadFile(filename[m], frame[m], m);
      //File does not exist -> no more frame -> replay animation
      if(remainingTime[m] == -1) {
        frame[m] = 0;
        remainingTime[m] = loadFile(filename[m], frame[m], m);
        if(remainingTime[m] == -1) {
          //Error, requested file does not exist
          //Serial.println("File does not exist: " + filename[m]);
          //reckeck as soon as possible:
          remainingTime[m] = 1;
        }
      }
      frame[m] ++;
      remainingTime[m] --;
      updateMatrix[m] = false;
    }
  }
  */
  /*
  if(Serial.available() > 0) {
    char buf[80];
    Serial.readBytesUntil('\n', buf, 80);
    if(String(buf).startsWith("/a")) {
      Serial.println("A!");
      Serial.println(String(buf).substring(2, sizeof(buf)));
    }
  }*/
  // if there's data available, read a packet
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    // read the packet into packetBufffer
    int len = Udp.read(packetBuffer, 255);
    if (len > 0) {
      packetBuffer[len] = 0;
    }
    //Serial.println(packetBuffer);
    parseCommand(packetBuffer);
  }
}

/*
 * Command syntax:
 * Play animation
 * /a <filename> <length>
 *  return <framenr> if frame it doesnt exist.
 * Send new frame:
 * /s <filename> <data>
 */
void parseCommand(char data[]) {
  if(String(data).startsWith("/a")) {
      String filename = splitString(String(data),' ', 1);
      int animationlength = splitString(String(data),' ', 2).toInt();
      for(int i = 0; i < animationlength; i ++) {
        if(!fileExists(filename + i + ".txt")) {
          //send a reply, to the IP address and port that sent us the packet we received
          Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
          dtostrf(i,4,0,ReplyBuffer);
          Udp.write(ReplyBuffer);
          Udp.endPacket();
        }
      }
  }
  if(String(data).startsWith("/s")) {
    String filename = splitString(String(data),' ', 1);
    String d = splitString(String(data),' ', 2);
    Serial.println("Writing file " + filename + " data: " + d);
  }
}

String splitString(String data, char separator, int index)
{
    int found = 0;
    int strIndex[] = { 0, -1 };
    int maxIndex = data.length() - 1;

    for (int i = 0; i <= maxIndex && found <= index; i++) {
        if (data.charAt(i) == separator || i == maxIndex) {
            found++;
            strIndex[0] = strIndex[1] + 1;
            strIndex[1] = (i == maxIndex) ? i+1 : i;
        }
    }
    return found > index ? data.substring(strIndex[0], strIndex[1]) : "";
}

int loadFile(String filename, uint8_t frame, uint8_t matrix_index) {
  readingFile = SD.open(filename + frame + ".txt");
  if(readingFile) {
    int duration = 0;
    uint8_t data[8];
    for(int i = 7; i >= 0; i --) {
      uint8_t nextchar = readingFile.read();
      if(nextchar == '0') {
        duration |= 0<<i;
      }
      else {
        duration |= 1<<i;
      }
    }
    for(uint8_t n = 0; n < 8; n ++) {
      data[n] = 0;
      for(int i = 7; i >= 0; i --) {
        uint8_t nextchar = readingFile.read();
        if(nextchar == '0') {
          data[n] |= 0<<i;
        }
        else {
          data[n] |= 1<<i;
        }
      }
    }
    readingFile.close();
    drawImage(data, matrix_index);
    return duration;
  }
  else {
    return -1;
  }
}

void drawImage(uint8_t data[], uint8_t m) {
  Serial.println("-----");
  for(int i = 0; i < 8; i ++) {
    Serial.println(data[i], BIN);
  }
  Serial.println("-----");
  matrix[m].clear();
  matrix[m].drawBitmap(0, 0, data, 8, 8, LED_ON);
  matrix[m].writeDisplay();
}

bool fileExists(String filename) {
  readingFile = SD.open(filename);
  if(readingFile) {
    return true;
  }
  else {
    return false;
  }
}

//WLAN:

void printMacAddress() {
  // the MAC address of your Wifi shield
  byte mac[6];                    

  // print your MAC address:
  WiFi.macAddress(mac);
  Serial.print("MAC: ");
  Serial.print(mac[5],HEX);
  Serial.print(":");
  Serial.print(mac[4],HEX);
  Serial.print(":");
  Serial.print(mac[3],HEX);
  Serial.print(":");
  Serial.print(mac[2],HEX);
  Serial.print(":");
  Serial.print(mac[1],HEX);
  Serial.print(":");
  Serial.println(mac[0],HEX);
}

void listNetworks() {
  // scan for nearby networks:
  Serial.println("** Scan Networks **");
  byte numSsid = WiFi.scanNetworks();

  // print the list of networks seen:
  Serial.print("number of available networks:");
  Serial.println(numSsid);

  // print the network number and name for each network found:
  for (int thisNet = 0; thisNet<numSsid; thisNet++) {
    Serial.print(thisNet);
    Serial.print(") ");
    Serial.print(WiFi.SSID(thisNet));
    Serial.print("\tSignal: ");
    Serial.print(WiFi.RSSI(thisNet));
    Serial.print(" dBm");
    Serial.print("\tEncryption: ");
    Serial.println(WiFi.encryptionType(thisNet));
  }
}



