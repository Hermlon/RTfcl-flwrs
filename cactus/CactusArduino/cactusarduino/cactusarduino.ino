#include <Adafruit_GFX.h>
#include "Adafruit_LEDBackpack.h"
#include "TimerOne.h"
#include <SD.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <Ethernet.h>
#include <EthernetUdp.h>
#include <SPI.h>

// Wifi only:
char ssid[] = "TP-LINK_A8FCC8";     // the name of your network
//char ssid[] = "dlink";
int status = WL_IDLE_STATUS;     // the Wifi radio's status
char pass[] = "59053491";    // your network key
WiFiUDP Udp;

//Ethernet only:
// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network:
byte mac[] = {
  0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED
};
IPAddress ip(10, 0, 0, 105);
int sendingport = 8051;
//EthernetUDP Udp;

unsigned int localPort = 2390;      // local port to listen on
char packetBuffer[UDP_TX_PACKET_MAX_SIZE]; //buffer to hold incoming packet

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
  Timer1.initialize(0.05 * 1000000);
  Timer1.attachInterrupt(timeBase);

  initWifi();
  //initEthernet();
  Udp.begin(localPort);
}

void initWifi() {
  //Init Wifi
   while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to WPA SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network:    
   status = WiFi.begin(ssid, pass);
    //status = WiFi.begin(ssid);
    // wait 10 seconds for connection:
    delay(10000);
  }
  Serial.print(WiFi.localIP());
  Serial.println(": You're connected to the network via wlan");
}

void initEthernet() {
   Ethernet.begin(mac, ip);
   Serial.println("You're connected to the network via ethernet");
}

void playAnimation(String name, uint8_t m) {
  filename[m] = name;
  remainingTime[m] = 0;
  frame[m] = 0;
}

void timeBase() {
  for(int m = 0; m < 5; m ++) {
    if(!updateMatrix[m]) {
      if(remainingTime[m] == 0) {
        updateMatrix[m] = true;
      }
      remainingTime[m] --;
    }
  }
}

void loop() {
  // if there's data available, read a packet
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    // read the packet into packetBufffer
    int len = Udp.read(packetBuffer, 255);
    if (len > 0) {
      packetBuffer[len] = 0;
    }
    Serial.print("New Package: ");
    Serial.println(packetBuffer);
    parseCommand(packetBuffer);
  }
  
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
  
  /*
  if(Serial.available() > 0) {
    char buf[80];
    Serial.readBytesUntil('\n', buf, 80);
    if(String(buf).startsWith("/a")) {
      Serial.println("A!");
      Serial.println(String(buf).substring(2, sizeof(buf)));
    }
  }*/
}

/*
 * Command syntax:
 * Play animation
 * /a <filename> <length> <matrix>
 *  return <framenr> if frame it doesnt exist.
 * Send new frame:
 * /s <filename> <data>
 */
void parseCommand(char data[]) {
  if(String(data).startsWith("/a")) {
      String filename = splitString(String(data),' ', 1);
      int animationlength = splitString(String(data),' ', 2).toInt();
      int matrixindex = splitString(String(data),' ', 3).toInt();
      bool incomplete = false;
      for(int i = 0; i < animationlength; i ++) {
        if(!fileExists(filename + String(i) + ".txt")) {
          incomplete = true;
          Serial.println("Frame " + String(i) + " of animation " + filename + " does not exist!");
          sendUDP("/g " + String(i));
        }
      }
      //Send end signal
      sendUDP("/e");
          
      if(!incomplete) {
        playAnimation(filename, matrixindex);
      }
  }
  if(String(data).startsWith("/s")) {
    Serial.println("/s cmd received");
    String filename = splitString(String(data),' ', 1);
    String d = splitString(String(data),' ', 2);
    if(d.length() == 72) {
        Serial.println("Writing file " + filename + " data: " + d);
        writeFile(filename, d);
        sendUDP("/n");
    }
    else {
      Serial.println("Invalid data length (" + String(d.length()) + ": " + filename + " | " + d);
    }
  }
}

void sendUDP(String text) {
   Udp.beginPacket(Udp.remoteIP(), sendingport);
   Udp.write(text.c_str());
   Serial.println("wrote <" + text +  ">");
   Udp.endPacket();
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
    duration = (int)(duration / 50);
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

void writeFile(String name, String data) {
  Serial.println("writeFile()");
  SD.remove(name);
  readingFile = SD.open(name, FILE_WRITE);
  if (readingFile) {
    readingFile.println(data);
    readingFile.close();
    Serial.println("Successfully wrote file: " + name);
  } else {
    Serial.println("error opening " + name);
  }
}

void drawImage(uint8_t data[], uint8_t m) {/*
  Serial.println("-----");
  for(int i = 0; i < 8; i ++) {
    Serial.println(data[i], BIN);
  }
  Serial.println("-----");*/
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

void printRemoteInfo() {
  IPAddress remote = Udp.remoteIP();
  for (int i = 0; i < 4; i++) {
      Serial.print(remote[i], DEC);
      if (i < 3) {
        Serial.print(".");
      }
    }
    Serial.print(", port ");
    Serial.println(Udp.remotePort());
}



