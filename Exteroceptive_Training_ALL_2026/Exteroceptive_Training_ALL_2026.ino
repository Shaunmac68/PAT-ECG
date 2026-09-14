#include <TimeLib.h>
#include <Arduino.h>
#include <Entropy.h>

#define CPU_RESTART_ADDR (uint32_t *)0xE000ED0C
#define CPU_RESTART_VAL 0x5FA0004
#define CPU_RESTART (*CPU_RESTART_ADDR = CPU_RESTART_VAL);

#define BUTTON_PIN_D0 0 // Pins 0 to 12 are the 13 bits that read data from the optical angle encoder/dial
#define BUTTON_PIN_D1 1
#define BUTTON_PIN_D2 2
#define BUTTON_PIN_D3 3
#define BUTTON_PIN_D4 4
#define BUTTON_PIN_D5 5
#define BUTTON_PIN_D6 6
#define BUTTON_PIN_D7 7
#define BUTTON_PIN_D8 8
#define BUTTON_PIN_D9 9
#define BUTTON_PIN_D10 10
#define BUTTON_PIN_D11 11
#define BUTTON_PIN_D12 12

#define BUTTON_PIN_D13 13 // LED on Teensy board
#define BUTTON_PIN_HR 14  // Heart R-wave pulse input
#define OUTPUT_PIN_AO 15 // Audio output signal pin (AO)
#define OUTPUT_PIN_VO 20 // Vibration output signal pin (VO)

//IntervalTimer myTimer1;
IntervalTimer Tone; //  Interrupt used to generate audio signal on OUTPUT_PIN_AO
IntervalTimer Vibration;  //  Interrupt used to generate vibration signal on OUTPUT_PIN_VO

char terminator = 13;

volatile bool ToneState = LOW;
volatile bool VibrationState = LOW;

volatile unsigned int toneWaveCount = 0;
volatile unsigned int vibrationWaveCount = 0;

int D[16];
int currentEncoderValue;
int lastEncoderValue;
int encoderShiftValue;

int toneDelayTime;
int lastToneDelayTime;
int tempToneDelayTime;

int HRP_Interval;
int HRP_Vals[7];  //********** Lesson learnt, don't add unsigned integers to signed, or you won't get the results you expect! **********
int HRP_Vals_Total;
int AveHRP;

int rrAve[3605];
int indexHRP;
  
const int HRP_DataPoints = 3;  //Number of HR values to average for output result

int trial;
int random1[100];

String training = "";

bool playTone = true;
bool exitEarly = false;

volatile bool flagHR;
volatile bool flagTone;
volatile bool flagVib;

unsigned long startTime;
unsigned long prevTime;
unsigned long currentTime;
unsigned long deltaT;

bool started;

void HR_Interrupt();
void randomGenerator();
void ToneFunction();
void VibrationFunction();
void readEncoder();

void setup() {
  Entropy.Initialize();
  // Randomise the noise seed using 'Entropy.random()'
  randomSeed(Entropy.random());

  // Define initial conditions and state variables
  int baseRR = 900 + random(0,201); // Set your desired baseline RR interval in ms between 900mS & 1100mS
  double hfState = 0.0;
  double lfState = 0.0;
  double vlfState = 0.0;
  double noiseState = 0.0;

  int rr[3605];

  // ***** CALCULATE SIMULATED NATURAL HEART BEAT PERIODIC TIMINGS *****
  // Run the loop exactly 3603 times (need the extra 3 for calculating averages of 3 later)
  for (int beatCount = 0; beatCount <= 3603; beatCount++) {

    // --- High-frequency component (drives RMSSD) ---
    hfState = 0.92 * hfState + ((random(0, 100000) / 100000.0) * 2.0 - 1.0) * 65.0;

    // --- Low-frequency component ---
    lfState = 0.985 * lfState + ((random(0, 100000) / 100000.0) * 2.0 - 1.0) * 18.0;

    // --- Very low frequency drift ---
    vlfState = 0.996 * vlfState + ((random(0, 100000) / 100000.0) * 2.0 - 1.0) * 6.0;

    // --- Short-term correlated noise ---
    noiseState = 0.90 * noiseState + ((random(0, 100000) / 100000.0) * 2.0 - 1.0) * 10.0;

    // --- Combine all components ---
    rr[beatCount] = baseRR + hfState + lfState + vlfState + noiseState;

    // --- Physiological bounds (avoid extreme/unrealistic values) ---
    if (rr[beatCount] < 550) {rr[beatCount] = 550;}
    if (rr[beatCount] > 1500) {rr[beatCount] = 1500;}

    // --- Output RR interval (ms) ---
    //Serial.println((int)rr);
  }

  //***** Calculate average of 3 HR periods ***** 
  for (int beatCount = 0; beatCount <= 3600; beatCount++) {
    rrAve[beatCount] = (rr[beatCount] + rr[beatCount + 1] + rr[beatCount + 2]) / 3;
  }
       
    pinMode(BUTTON_PIN_D0, INPUT_PULLUP); //  Set pins as input
    pinMode(BUTTON_PIN_D1, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D2, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D3, INPUT_PULLUP); 
    pinMode(BUTTON_PIN_D4, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D5, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D6, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D7, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D8, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D9, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D10, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D11, INPUT_PULLUP);
    pinMode(BUTTON_PIN_D12, INPUT_PULLUP);

    pinMode(BUTTON_PIN_D13, OUTPUT); //  Set pin as output
    pinMode(OUTPUT_PIN_AO, OUTPUT);
    pinMode(OUTPUT_PIN_VO, OUTPUT);

    pinMode(BUTTON_PIN_HR, INPUT);

    for (int i = 1; i <= HRP_DataPoints; i++){HRP_Vals[i] = 1000;} //*** Reset heart rate period to 1000mS / 60BPM ***
    
    Tone.begin(ToneFunction, 500); // 500 or state change every 0.5mS, giving output on D15 of 1KHz
    Vibration.begin(VibrationFunction, 10000); // 10000 or state change every 10mS, giving output on D16 of 50Hz

    flagHR == false;
    
    attachInterrupt(digitalPinToInterrupt(BUTTON_PIN_HR), HR_Interrupt, RISING); // attach interrupt to BUTTON_PIN_HR
    
    Serial.begin(2000000);
    //while(!Serial);
    delay(100);
    
    //for (int beatCount = 1; beatCount <= 3600; beatCount++){
    //  Serial.println(rrAve[beatCount]);
    //}

    randomGenerator();
      
    do {
      String incomingString = Serial.readString(); //readStringUntil(terminator);  // will not be -1
      //String junk = Serial.readString();
      
      if (incomingString.indexOf("AYTR1") >= 0){ //  Run this if 'Training' mode
        Serial.println("OK");
        flashLED(5);
        training = "yes1";
        trial = 0;
        break;  
      }
      if (incomingString.indexOf("AYTR2") >= 0){ //  Run this if 'Training' mode
        Serial.println("OK");
        flashLED(5);
        training = "yes2";
        trial = 0;
        break;  
      }
      if (incomingString.indexOf("AYTR3") >= 0){ //  Run this if 'Training' mode
        Serial.println("OK");
        flashLED(5);
        training = "yes3";
        trial = 0;
        break;  
      }

      if (incomingString.indexOf("AYNT1") >= 0){ //  Run this if not 'Training' mode
        Serial.println("OK");
        flashLED(5); 
        training = "no1";
        trial = 0;
        started = true;
        flagHR = false;
        break; 
      }
      if (incomingString.indexOf("AYNT2") >= 0){ //  Run this if not 'Training' mode
        Serial.println("OK");
        flashLED(5); 
        training = "no2";
        trial = 0;
        started = true;
        flagHR = false;
        break; 
      }
    } while (Serial.available() == 0) ;
    prevTime = millis();
}

void ToneFunction() {
  if (ToneState == LOW) {
    ToneState = HIGH;
  } else {
    ToneState = LOW;
    toneWaveCount += 1;
  }
  digitalWriteFast(OUTPUT_PIN_AO, ToneState); //  Toggle audio output pin ...

  if (toneWaveCount >= 20) { //  ... for 20mS (tone duration changed from 200mS on 26/05/2026)
  toneWaveCount=0;
  Tone.end();
  } 
}

void VibrationFunction() {
  if (VibrationState == LOW) {
    VibrationState = HIGH;
  } else {
    VibrationState = LOW;
    vibrationWaveCount += 1;
  }
  digitalWriteFast(OUTPUT_PIN_VO, VibrationState); //  Toggle vibration output pin ...

  if (vibrationWaveCount >= 1) { //  ... for 20mS
  vibrationWaveCount=0;
  Vibration.end();
  }
}

void loop() {
  if (training == "yes1"){ //  ***** TRAINING MODE *****
    readEncoder();
    toneDelayTime = currentEncoderValue / 8.192; //  toneDelayTime is set from 0 to 1000mS with the dial. Dial position sets D0-D13 to binary value between 0 and 8192, hence toneDelayTime = encoderValue / 8.192

    if (toneDelayTime >= 1000){toneDelayTime = 1000;} // ***** Added to make sure toneDelayTime doesn't exceed 1000mS (PROBABLY NOT REQUIRED!) *****

    //  If (toneDelayTime < 200), vibration starts (200 - toneDelayTime)mS after the tone starts to play.
    //  If (toneDelayTime = 200), vibration & tone start at the same time.
    //  If (toneDelayTime > 200), tone starts to play (200 - toneDelayTime)mS after the vibration starts.
    
    // ***** THIS HAS BEEN DONE SO THAT THERE IS SOME FLEXIBILITY. FOR 20% OF THE DIAL MOVEMENT, THE TONE WILL START BEFORE THE TACTILE STIMULUS. FOR THE REMAINING 80%, THE VIBRATION WILL START BEFORE THE TONE. ALL WHILST IN TRAINING MODE.
        
    if (toneDelayTime <= 200){
      if (playTone == true){Tone.begin(ToneFunction, 500);} // 500uS or state change every 0.5mS, giving output on pin 'AO' of 1KHz for 200mS
      delayMicroseconds((200 - toneDelayTime) * 1000);
      digitalWrite(BUTTON_PIN_D13, HIGH);
      if (playTone == true){Vibration.begin(VibrationFunction, 10000);} // 10000 or state change every 10mS, giving output on pin 'VO' of 50Hz for 200mS
      delayMicroseconds((800 + toneDelayTime) * 1000);
      digitalWrite(BUTTON_PIN_D13, LOW);
      //delayMicroseconds(200 * 1000);
    }
    if (toneDelayTime > 200){ //  If toneDelayTime > 200mS, start vibration, followed by tone toneDelayTime-200mS later
      //digitalWrite(BUTTON_PIN_D13, HIGH);
      if (playTone == true){Vibration.begin(VibrationFunction, 10000);} // 10000 or state change every 10mS, giving output on D16 of 50Hz
      delayMicroseconds((toneDelayTime - 200) * 1000);
  
      if (playTone == true){Tone.begin(ToneFunction, 500);} // 500uS or state change every 0.5mS, giving output on D15 of 1KHz
      delayMicroseconds((1200 - toneDelayTime) * 1000);
      digitalWrite(BUTTON_PIN_D13, LOW);
      //delayMicroseconds(200 * 1000);
    }
    Serial.print(toneDelayTime - 200);
    Serial.println(", 0, 0");
  } 
  if (training == "yes2"){ //  ***** TRAINING MODE *****
    startTime = millis();
    if (playTone == true){Vibration.begin(VibrationFunction, 10000);} // 10000 or state change every 10mS, giving output on D16 of 50Hz

    indexHRP += 1;
    readEncoder();

    if (currentEncoderValue >= lastEncoderValue)
    {
      if ((currentEncoderValue - lastEncoderValue) > 4096)
      {
        encoderShiftValue = currentEncoderValue - lastEncoderValue - 8192;
      }else{
        encoderShiftValue = currentEncoderValue - lastEncoderValue;
      }
    }else{
      if ((currentEncoderValue - lastEncoderValue) < -4096)
      {
        encoderShiftValue = currentEncoderValue - lastEncoderValue + 8192;
      }else{
        encoderShiftValue = currentEncoderValue - lastEncoderValue;
      }
    }
      
    toneDelayTime = toneDelayTime + (encoderShiftValue * 800) / 8192; //*****  6.82 = 1000 / 1200 * 8.192  ***** USE the calculated average HRP to set the range of the encoder/dial, i.e. from 0 to AveHRP value *****

    lastEncoderValue = currentEncoderValue; // Store current encoder value for next time, so that one can detect a shift in the encoder dial position
      
    if (toneDelayTime >= rrAve[indexHRP]){
      toneDelayTime = toneDelayTime - rrAve[indexHRP]; // *** Make sure that toneDelayTime, set by encoder/dial position, isn't greater than the last HRP value. If it is, set it to last HRP value ***
    }
    if (toneDelayTime < 0){  
      toneDelayTime = toneDelayTime + rrAve[indexHRP]; // *** Make sure that is toneDelayTime <0, set toneDelayTime to last HRP minus the negative value of toneDelayTime ***
    }
   
    do{
        delayMicroseconds(100);
        currentTime = millis();
      } 
    while ((currentTime - startTime) < toneDelayTime);
        
    if ((playTone == true) && ((toneDelayTime - lastToneDelayTime) >= -300)){Tone.begin(ToneFunction, 500);} // 500uS or state change every 0.5mS, giving output on D15 of 1KHz

    lastToneDelayTime = toneDelayTime;
          
    delayMicroseconds((rrAve[indexHRP] - toneDelayTime) * 1000);
    
    Serial.print(toneDelayTime);
    Serial.print(",");
    Serial.print(rrAve[indexHRP]);
    Serial.println(", 0");
  }
  if (training == "yes3"){ //  ***** TRAINING MODE *****

    indexHRP += 1;
    readEncoder();

    if (currentEncoderValue >= lastEncoderValue)
    {
      if ((currentEncoderValue - lastEncoderValue) > 4096)
      {
        encoderShiftValue = currentEncoderValue - lastEncoderValue - 8192;
      }else{
        encoderShiftValue = currentEncoderValue - lastEncoderValue;
      }
    }else{
      if ((currentEncoderValue - lastEncoderValue) < -4096)
      {
        encoderShiftValue = currentEncoderValue - lastEncoderValue + 8192;
      }else{
        encoderShiftValue = currentEncoderValue - lastEncoderValue;
      }
    }
      
    toneDelayTime = toneDelayTime + (encoderShiftValue * 800) / 8192; //*****  6.82 = 1000 / 1200 * 8.192  ***** USE the calculated average HRP to set the range of the encoder/dial, i.e. from 0 to AveHRP value *****

    lastEncoderValue = currentEncoderValue; // Store current encoder value for next time, so that one can detect a shift in the encoder dial position
      
    if (toneDelayTime >= (rrAve[indexHRP] / 2)){
      toneDelayTime = toneDelayTime - rrAve[indexHRP]; // *** Make sure that toneDelayTime, set by encoder/dial position, isn't greater than the last HRP value. If it is, set it to last HRP value ***
    }
    if (toneDelayTime < (-rrAve[indexHRP] / 2)){  
      toneDelayTime = toneDelayTime + rrAve[indexHRP]; // *** Make sure that is toneDelayTime <0, set toneDelayTime to last HRP minus the negative value of toneDelayTime ***
    }

    startTime = millis();

    if (toneDelayTime < 0){
      if (playTone == true){Tone.begin(ToneFunction, 500);} // 500uS or state change every 0.5mS, giving output on D15 of 1KHz
    }else{
      if (playTone == true){Vibration.begin(VibrationFunction, 10000);} // 10000 or state change every 10mS, giving output on D16 of 50Hz
    }
   
    do{
        delayMicroseconds(100);
        currentTime = millis();
      } 
    while ((currentTime - startTime) < abs(toneDelayTime));

    if (toneDelayTime < 0){
      if (playTone == true){Vibration.begin(VibrationFunction, 10000);} // 10000 or state change every 10mS, giving output on D16 of 50Hz
    }else{
      if (playTone == true){Tone.begin(ToneFunction, 500);} // 500uS or state change every 0.5mS, giving output on D15 of 1KHz
    }

    lastToneDelayTime = toneDelayTime;
          
    delayMicroseconds((rrAve[indexHRP] - abs(toneDelayTime)) * 1000);
    
    Serial.print(toneDelayTime);
    Serial.print(",");
    Serial.print(rrAve[indexHRP]);
    Serial.print(",");
    Serial.println(rrAve[indexHRP] - abs(toneDelayTime));
  }

  if (training == "no1"){  //  ***** REAL TRIAL MODE *****
    if (flagHR == true){  // Run this if 'R' wave signal received
      flagHR = false;
          
      if (exitEarly != true){  // If exitEarly is TRUE then startTime has already been set due to next R wave arriving early! See do-while loop below...
        startTime = millis();
      }
      else{exitEarly = false;}  //Reset exitEarly flag ready for loop below...       

      digitalWriteFast(BUTTON_PIN_D13,HIGH);
                 
      if (started != true){
        HRP_Interval = startTime - prevTime;
      }else{
        HRP_Interval = 1000;  //  Assume heart rate is 60BPM if there is no data to work with yet, then fill HRP[] array with 1000mS values.
        for (int i = 1; i <= HRP_DataPoints; i++){HRP_Vals[i] = 1000;}
        started = false; 
      }

      //  *** CALCULATES MOVING AVERAGE OF HRP VALUES ***
      HRP_Vals[HRP_DataPoints] = HRP_Interval;  //  Set last HRP[] array element to latest HRP.
      HRP_Vals_Total = 0;

      for (int i = 1; i < HRP_DataPoints; i++)
      {
          HRP_Vals_Total = HRP_Vals_Total + HRP_Vals[i]; //*** Add the last HRP_DataPoints-1 HRP values to the total ***
          HRP_Vals[i] = HRP_Vals[i+1];    //*** Shift the array elements down one ***
      }
      
      HRP_Vals_Total = HRP_Vals_Total + HRP_Interval;  //*** Now add the latest HRP value to the total ***
      
      AveHRP = HRP_Vals_Total / HRP_DataPoints; //*** Calculate the average heart rate period over the last HRP_DataPoints-1 samples ***
      
      readEncoder();  //  READ ENCODER ANGLE BINARY DATA, CONVERT FROM GREY CODE TO DIGITAL, THEN OFFSET STARTING POSITION WITH PSEUDO RANDOM VALUE FROM 1 OF 4 POSITIONS (PLEASE SEE 'readEncoder' SUB-ROUTINE AT BOTTOM OF LISTING).
      
      toneDelayTime = (currentEncoderValue * AveHRP) / 8192; //*****  6.82 = 1000 / 1200 * 8.192  ***** USE the calculated average HRP to set the range of the encoder/dial, i.e. from 0 to AveHRP value *****
      
      if (toneDelayTime > (HRP_Interval)) {toneDelayTime = HRP_Interval;} // - 20;}  // *** Make sure that toneDelayTime, set by encoder/dial position, isn't greater than the last HRP value. If it is, set it to last HRP value ***

      //********** STORE 'startTime' FOR USE IN NEXT R WAVE ARRIVAL to calculate R-R period **********
      prevTime = startTime; 
      //**********************************************************************************************
      
      //********** Loop to produce a delay of length 'toneDelayTime'. If new R wave arrives, break from loop & store startTime! **********
      do{
        delayMicroseconds(100);
        currentTime = millis();
        if (flagHR == true){ 
          exitEarly = true;
          toneDelayTime = currentTime - startTime;  //*** R WAVE HAS ARRIVED TOO EARLY, SO MAKE 'toneDelayTime' = LAST R-R PERIOD! ***
          startTime = currentTime;
          break;
        }
      } 
      while ((currentTime - startTime) < toneDelayTime);
      //**********************************************************
      
      digitalWriteFast(BUTTON_PIN_D13,LOW);
      if ((playTone == true) && ((toneDelayTime + (AveHRP - lastToneDelayTime)) >= 300)){Tone.begin(ToneFunction, 500);} // 500 or state change every 0.5mS, giving output on AO of 1KHz

      //********** STORE 'lastToneDelayTime' as the last tone delay time, to assist with avoiding double beep! *****
      lastToneDelayTime = toneDelayTime;
      
      Serial.print(toneDelayTime);  //  *** Output data to serial port ***
      Serial.print(", ");             
      Serial.print(HRP_Interval);
      Serial.print(", ");                        
      Serial.println(AveHRP);
      //Serial.print(", ");                        
      //Serial.println (exitEarly);
    }
  }
  if (training == "no2"){  //  ***** REAL TRIAL MODE *****
    if (flagHR == true){  // Run this if 'R' wave signal received
      flagHR = false;
          
      if (exitEarly != true){  // If exitEarly is TRUE then startTime has already been set due to next R wave arriving early! See do-while loop below...
        startTime = millis();
      }
      else{exitEarly = false;}  //Reset exitEarly flag ready for loop below...       

      digitalWriteFast(BUTTON_PIN_D13,HIGH);
                 
      if (started != true){
        HRP_Interval = startTime - prevTime;
      }else{
        HRP_Interval = 1000;  //  Assume heart rate is 60BPM if there is no data to work with yet, then fill HRP[] array with 1000mS values.
        for (int i = 1; i <= HRP_DataPoints; i++){HRP_Vals[i] = 1000;}
        started = false; 
      }

      //  *** CALCULATES MOVING AVERAGE OF HRP VALUES ***
      HRP_Vals[HRP_DataPoints] = HRP_Interval;  //  Set last HRP[] array element to latest HRP.
      HRP_Vals_Total = 0;

      for (int i = 1; i < HRP_DataPoints; i++)
      {
          HRP_Vals_Total = HRP_Vals_Total + HRP_Vals[i]; //*** Add the last HRP_DataPoints-1 HRP values to the total ***
          HRP_Vals[i] = HRP_Vals[i+1];    //*** Shift the array elements down one ***
      }
      
      HRP_Vals_Total = HRP_Vals_Total + HRP_Interval;  //*** Now add the latest HRP value to the total ***
      
      AveHRP = HRP_Vals_Total / HRP_DataPoints; //*** Calculate the average heart rate period over the last HRP_DataPoints-1 samples ***
      
      readEncoder();  //  READ ENCODER ANGLE BINARY DATA, CONVERT FROM GREY CODE TO DIGITAL, THEN OFFSET STARTING POSITION WITH PSEUDO RANDOM VALUE FROM 1 OF 4 POSITIONS (PLEASE SEE 'readEncoder' SUB-ROUTINE AT BOTTOM OF LISTING).

      if (currentEncoderValue >= lastEncoderValue)
      {
        if ((currentEncoderValue - lastEncoderValue) > 4096)
        {
          encoderShiftValue = currentEncoderValue - lastEncoderValue - 8192;
        }else{
          encoderShiftValue = currentEncoderValue - lastEncoderValue;
        }
      }else{
        if ((currentEncoderValue - lastEncoderValue) < -4096)
        {
          encoderShiftValue = currentEncoderValue - lastEncoderValue + 8192;
        }else{
          encoderShiftValue = currentEncoderValue - lastEncoderValue;
        }
      }
      
      toneDelayTime = toneDelayTime + (encoderShiftValue * 800) / 8192; //*****  6.82 = 1000 / 1200 * 8.192  ***** USE the calculated average HRP to set the range of the encoder/dial, i.e. from 0 to AveHRP value *****

      lastEncoderValue = currentEncoderValue; // Store current encoder value for next time, so that one can detect a shift in the encoder dial position
      
      if (toneDelayTime >= AveHRP){
        toneDelayTime = toneDelayTime - AveHRP; // *** Make sure that toneDelayTime, set by encoder/dial position, isn't greater than the last HRP value. If it is, set it to last HRP value ***
      }
      if (toneDelayTime < 0){  
        toneDelayTime = toneDelayTime + AveHRP; // *** Make sure that is toneDelayTime <0, set toneDelayTime to last HRP minus the negative value of toneDelayTime ***
      }

      tempToneDelayTime = toneDelayTime; //This is used as a temporary variable for toneDelayTime, see below...
      
      //********** STORE 'startTime' FOR USE IN NEXT R WAVE ARRIVAL to calculate R-R period **********
      prevTime = startTime;
          
      //********** Loop to produce a delay of length 'toneDelayTime'. If new R wave arrives, break from loop & store startTime! **********
      do{
        delayMicroseconds(100);
        currentTime = millis();
        if (flagHR == true){ 
          exitEarly = true;
          tempToneDelayTime = currentTime - startTime;  //*** R WAVE HAS ARRIVED TOO EARLY, SO MAKE 'tempToneDelayTime' = LAST R-R PERIOD! ***
          startTime = currentTime;
          break;
        }
      } 
      while ((currentTime - startTime) < tempToneDelayTime);
      //**********************************************************
      
      digitalWriteFast(BUTTON_PIN_D13,LOW);

      //***** Check to see if tone should be played AND that the difference in tone delay periods is greater than -500mS *****      
      if ((playTone == true) && ((toneDelayTime + (AveHRP - lastToneDelayTime)) >= 300)){Tone.begin(ToneFunction, 500);} // 500 or state change every 0.5mS, giving output on AO of 1KHz

      //********** STORE 'lastToneDelayTime' as the last tone delay time, to assist with avoiding double beep! *****
      lastToneDelayTime = toneDelayTime;

      Serial.print(tempToneDelayTime);  //  *** Output data to serial port ***
      Serial.print(", ");             
      Serial.print(HRP_Interval);
      Serial.print(", ");                        
      Serial.println(AveHRP);
      //Serial.print(", ");                        
      //Serial.println (exitEarly);
    }
  }

  int k = Serial.read();// *** Check to see if any serial data has been received from PC. If so, run the code below ***
  
  if (k >= 0) {
    String incomingString = char(k) + Serial.readString();  // Read all data & add first character to string

    if (incomingString.indexOf("AYTR1") >= 0){ //*** RUN TRAINING MODE ***
      Serial.flush();
      flashLED(3);
      Serial.println("OK");
      randomGenerator(); // ***** CALL RANDOM NUMBER GENERATOR ROUTINE, needed for shifting of dial starting position *****
      trial = 0;
      training = "yes1";
      indexHRP = 0;
    }
    if (incomingString.indexOf("AYTR2") >= 0){ //*** RUN TRAINING MODE ***
      Serial.flush();
      flashLED(3);
      Serial.println("OK");
      randomGenerator(); // ***** CALL RANDOM NUMBER GENERATOR ROUTINE, needed for shifting of dial starting position *****
      trial = 0;
      training = "yes2";
      indexHRP = 0;
    }
    if (incomingString.indexOf("AYTR3") >= 0){ //*** RUN TRAINING MODE ***
      Serial.flush();
      flashLED(3);
      Serial.println("OK");
      randomGenerator(); // ***** CALL RANDOM NUMBER GENERATOR ROUTINE, needed for shifting of dial starting position *****
      trial = 0;
      training = "yes3";
      indexHRP = 0;
    }
    
    if (incomingString.indexOf("AYNT1") >= 0){ //*** RUN REAL TRIAL MODE ***
      Serial.flush();
      flashLED(3);
      Serial.println("OK");
      randomGenerator(); // ***** CALL RANDOM NUMBER GENERATOR ROUTINE, needed for shifting of dial starting position *****
      trial = 0;
      started = true;
      training = "no1";
      flagHR = false;
    }
    if (incomingString.indexOf("AYNT2") >= 0){ //*** RUN REAL TRIAL MODE ***
      Serial.flush();
      flashLED(3);
      Serial.println("OK");
      randomGenerator(); // ***** CALL RANDOM NUMBER GENERATOR ROUTINE, needed for shifting of dial starting position *****
      trial = 0;
      started = true;
      training = "no2";
      flagHR = false;
    }
    
    if (incomingString.indexOf("AYON") >= 0){ // Start next trial
      Serial.flush();
      trial++;      
      playTone = true;
      exitEarly == false;
      started = true;
      flagHR = false;     
    }
    if (incomingString.indexOf("AYOF") >= 0){ // Stop data collection
      Serial.flush();
      playTone = false;
    }
    if (incomingString.indexOf("AYT0") >= 0){ // Reset trial number to '1'
      Serial.flush();
      trial = 1;
      playTone = true;
    }
    if (incomingString.indexOf("AYEX") >= 0){ //*** Make Teensy board restart *** 
      CPU_RESTART; // restart CPU
    }
    //Serial.println("End");           
  }
}

void flashLED(int z){
  for (int i = 1; i <= z; i++)  // Make the LED flash on the Teensy board 'z' times
  {
    digitalWriteFast(BUTTON_PIN_D13, HIGH);
    delay(25);
    digitalWriteFast(BUTTON_PIN_D13, LOW); 
    delay(25);
  }
}

void randomGenerator(){ //***** Generate pseudo random numbers from 1 to 4 with no repeats. For example, 3.4.1.2,4,1,3,2,3,2,4,1,2,4,3,1... *****
  int rndPtr;
  random1[0] = 0;
  for (int lp = 1; lp <= 44; lp = lp + 4){
    do {
      random1[lp] = 0;
      random1[lp + 1] = 0;
      random1[lp + 2] = 0;
      random1[lp + 3] = 0;
      for (int lp2 = 0; lp2 < 4; lp2 = lp2 + 1){     
        do
          {rndPtr = lp + random (0,4);}
        while (random1[rndPtr] > 0);
        random1[rndPtr] = lp2 + 1;
      }
    }
    while (random1[lp] == random1[lp - 1]);
  }
}

void readEncoder(){ //***** READ ENCODER ANGLE BINARY DATA, CONVERT FROM GREY CODE TO DECIMAL, THEN OFFSET STARTING POSITION OF DIAL/ENCODER WITH PSEUDO RANDOM VALUE FROM 1 OF 4 POSITIONS, THEN RETURN VALUE *****
    D[0] = digitalReadFast(BUTTON_PIN_D12);
    D[1] = digitalReadFast(BUTTON_PIN_D11);
    D[2] = digitalReadFast(BUTTON_PIN_D10);
    D[3] = digitalReadFast(BUTTON_PIN_D9);
    D[4] = digitalReadFast(BUTTON_PIN_D8);
    D[5] = digitalReadFast(BUTTON_PIN_D7);
    D[6] = digitalReadFast(BUTTON_PIN_D6);
    D[7] = digitalReadFast(BUTTON_PIN_D5);
    D[8] = digitalReadFast(BUTTON_PIN_D4);
    D[9] = digitalReadFast(BUTTON_PIN_D3);
    D[10] = digitalReadFast(BUTTON_PIN_D2);
    D[11] = digitalReadFast(BUTTON_PIN_D1);
    D[12] = digitalReadFast(BUTTON_PIN_D0);

    //*** GRAY CODE TO DECIMAL CONVERSION CODE ***
    int value = 0;
    
    if (D[12] == 1){value = value + pow(2,12);}

    for (byte i=12; i>0; i--)
    {
      D[i-1] = D[i] ^ D[i-1];
      if (D[i-1] == 1 ){
        value = value + pow(2,(i-1));
      }
    }
    //*** END OF GRAY CODE TO DECIMAL CONVERSION CODE ***

    //*** SET DIAL / ENCODER STARTING POSITION TO 1 OF 4 PSEUDO-RANDOM POSITIONS
    
    //if (random1[trial] == 1){value = value;}
    if (random1[trial] == 2){value -= 2048;}
    if (random1[trial] == 3){value -= 4096;}
    if (random1[trial] == 4){value -= 6144;}
    if (value < 0){value += 8192;}
    currentEncoderValue = value; // RETURN ENCODER/DIAL VALUE
}

void HR_Interrupt() { // SET FLAG IF R-WAVE PULSE RECEIVED BY TEENSY BOARD
  flagHR = true;
}
