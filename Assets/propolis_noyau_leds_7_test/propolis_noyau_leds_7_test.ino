



// hardware libraries to access use the shield

#include <Ethernet.h>
#include <EthernetUdp.h>
#include <SPI.h>
#include <OSCBundle.h>
#include <OSCBoards.h>
#include "FastLED.h"

#define LED_PIN    22
#define COLOR_ORDER GRB
#define CHIPSET     WS2811
#define NUM_LEDS    84
#define NUM_STRIPS 7
#define BRIGHTNESS  50
#define FRAMES_PER_SECOND 10

EthernetUDP Udp;
byte mac[] = {
  0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED
};
IPAddress ip(192, 168, 0, 3);
CRGB leds[NUM_STRIPS][NUM_LEDS];
const unsigned int inPort = 12001;
int currentId = 0;


void setup() {



  Serial.begin(9600);
  //setup ethernet part
  Ethernet.begin(mac, ip);
  Udp.begin(inPort);

  Serial.println(Ethernet.localIP());

  delay(3000); // sanity delay

  // Settings up the led strips
  FastLED.addLeds<CHIPSET, 22, COLOR_ORDER>(leds[0], NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.addLeds<CHIPSET, 24, COLOR_ORDER>(leds[1], NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.addLeds<CHIPSET, 26, COLOR_ORDER>(leds[2], NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.addLeds<CHIPSET, 28, COLOR_ORDER>(leds[3], NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.addLeds<CHIPSET, 30, COLOR_ORDER>(leds[4], NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.addLeds<CHIPSET, 32, COLOR_ORDER>(leds[5], NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.addLeds<CHIPSET, 34, COLOR_ORDER>(leds[6], NUM_LEDS).setCorrection( TypicalLEDStrip );

  FastLED.setBrightness( BRIGHTNESS );

  testPattern();



}

void loop() {
  // important! non-blocking listen routine
  OSCBundle bundleIN;
  int size;

  if ( (size = Udp.parsePacket()) > 0)
  {

    while (size--)
      bundleIN.fill(Udp.read());
    Serial.println(bundleIN.hasError());

  }
  //if(!bundleIN.hasError()){
  bundleIN.route("/id", receiveID);
  bundleIN.route("/status", receiveStatus);
  // }



 //testPattern();

}


void receiveID(OSCMessage &msg) {  // *note the & before msg
  // getting to the message data
  Serial.println("id" + String(msg.getInt(0)));

  currentId = msg.getInt(0);

}

void receiveStatus(OSCMessage &msg) {  // *note the & before msg
  Serial.println("status" + String(msg.getInt(0)));
  //Testin on 7 leds only
  /*switch(msg.getInt(0)){
    case 0: leds[currentId] = CRGB::Black;    // Black/off
            break;
    case 1: leds[currentId] = CRGB::White;
            break;
    case 2: leds[currentId] = CRGB::Red;
            break;
    }*/

  switch (msg.getInt(0)) {
    case 0: colorWipe(CRGB::Black);    // Black/off
      break;
    case 1: colorWipe(CRGB::White);
      break;
    case 2: colorWipe(CRGB::Red);
      break;
  }


}

void testPattern() {
  for (int i = 0; i < 6; i ++) {
    currentId = i;
    colorWipe(CRGB::White);
  }


  delay(500);

  for (int i = 0; i < 6; i ++) {
    currentId = i;
    colorWipe(CRGB::Black);
  }

  currentId = 0;
  colorWipe(CRGB::Red);


  delay(500);
  currentId = 1;
  colorWipe(CRGB::Green);


  delay(500);
  currentId = 2;
  colorWipe(CRGB::Red);

   delay(500);
  currentId = 3;
  colorWipe(CRGB::Green);

   delay(500);
  currentId = 4;
  colorWipe(CRGB::Red);

  delay(500);
  currentId = 5;
  colorWipe(CRGB::Green);

  
   delay(500);
  currentId = 6;
  colorWipe(CRGB::Red);

  delay(500);
  for (int i = 0; i < 6; i ++) {
    currentId = i;
    colorWipe(CRGB::Black);
  }

}


// Fill the dots one after the other with a color
void colorWipe(CRGB color) {
  for (uint16_t i = 0; i < NUM_LEDS; i++) {
    leds[currentId][i] = color;
  }
  FastLED.show();
}

