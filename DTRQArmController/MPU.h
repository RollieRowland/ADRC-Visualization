#pragma once

#include "helper_3dmath.h"
#include <inttypes.h>
#include <stddef.h>

class MPU {
public:
	MPU() { }
	virtual ~MPU() {};
	MPU(const MPU& mpu) { *this = mpu; }

	virtual void initialize() = 0;
    virtual bool testConnection() = 0;

    // AUX_VDDIO register
    virtual uint8_t getAuxVDDIOLevel() = 0;
    virtual void setAuxVDDIOLevel(uint8_t level) = 0;

    // SMPLRT_DIV register
	virtual uint8_t getRate() = 0;
	virtual void setRate(uint8_t rate) = 0;

    // CONFIG register
	virtual uint8_t getExternalFrameSync() = 0;
	virtual void setExternalFrameSync(uint8_t sync) = 0;
	virtual uint8_t getDLPFMode() = 0;
	virtual void setDLPFMode(uint8_t bandwidth) = 0;

    // GYRO_CONFIG register
    virtual uint8_t getFullScaleGyroRange() = 0;
    virtual void setFullScaleGyroRange(uint8_t range) = 0;

	// SELF_TEST registers
	virtual uint8_t getAccelXSelfTestFactoryTrim() = 0;
	virtual uint8_t getAccelYSelfTestFactoryTrim() = 0;
	virtual uint8_t getAccelZSelfTestFactoryTrim() = 0;

	virtual uint8_t getGyroXSelfTestFactoryTrim() = 0;
	virtual uint8_t getGyroYSelfTestFactoryTrim() = 0;
	virtual uint8_t getGyroZSelfTestFactoryTrim() = 0;
		
    // ACCEL_CONFIG register
	virtual bool getAccelXSelfTest() = 0;
	virtual void setAccelXSelfTest(bool enabled) = 0;
	virtual bool getAccelYSelfTest() = 0;
	virtual void setAccelYSelfTest(bool enabled) = 0;
	virtual bool getAccelZSelfTest() = 0;
	virtual void setAccelZSelfTest(bool enabled) = 0;
	virtual uint8_t getFullScaleAccelRange() = 0;
	virtual void setFullScaleAccelRange(uint8_t range) = 0;
    virtual uint8_t getDHPFMode() = 0;
	virtual void setDHPFMode(uint8_t mode) = 0;

    // FF_THR register
	virtual uint8_t getFreefallDetectionThreshold() = 0;
	virtual void setFreefallDetectionThreshold(uint8_t threshold) = 0;

    // FF_DUR register
	virtual uint8_t getFreefallDetectionDuration() = 0;
	virtual void setFreefallDetectionDuration(uint8_t duration) = 0;

    // MOT_THR register
	virtual uint8_t getMotionDetectionThreshold() = 0;
    virtual void setMotionDetectionThreshold(uint8_t threshold) = 0;

    // MOT_DUR register
	virtual uint8_t getMotionDetectionDuration() = 0;
	virtual void setMotionDetectionDuration(uint8_t duration) = 0;

    // ZRMOT_THR register
	virtual uint8_t getZeroMotionDetectionThreshold() = 0;
	virtual void setZeroMotionDetectionThreshold(uint8_t threshold) = 0;

    // ZRMOT_DUR register
	virtual uint8_t getZeroMotionDetectionDuration() = 0;
	virtual void setZeroMotionDetectionDuration(uint8_t duration) = 0;

    // FIFO_EN register
	virtual bool getTempFIFOEnabled() = 0;
	virtual void setTempFIFOEnabled(bool enabled) = 0;
	virtual bool getXGyroFIFOEnabled() = 0;
	virtual void setXGyroFIFOEnabled(bool enabled) = 0;
	virtual bool getYGyroFIFOEnabled() = 0;
	virtual void setYGyroFIFOEnabled(bool enabled) = 0;
	virtual bool getZGyroFIFOEnabled() = 0;
	virtual void setZGyroFIFOEnabled(bool enabled) = 0;
	virtual bool getAccelFIFOEnabled() = 0;
	virtual void setAccelFIFOEnabled(bool enabled) = 0;
	virtual bool getSlave2FIFOEnabled() = 0;
	virtual void setSlave2FIFOEnabled(bool enabled) = 0;
	virtual bool getSlave1FIFOEnabled() = 0;
	virtual void setSlave1FIFOEnabled(bool enabled) = 0;
	virtual bool getSlave0FIFOEnabled() = 0;
	virtual void setSlave0FIFOEnabled(bool enabled) = 0;

    // I2C_MST_CTRL register
	virtual bool getMultiMasterEnabled() = 0;
	virtual void setMultiMasterEnabled(bool enabled) = 0;
	virtual bool getWaitForExternalSensorEnabled() = 0;
	virtual void setWaitForExternalSensorEnabled(bool enabled) = 0;
	virtual bool getSlave3FIFOEnabled() = 0;
	virtual void setSlave3FIFOEnabled(bool enabled) = 0;
	virtual bool getSlaveReadWriteTransitionEnabled() = 0;
	virtual void setSlaveReadWriteTransitionEnabled(bool enabled) = 0;
	virtual uint8_t getMasterClockSpeed() = 0;
	virtual void setMasterClockSpeed(uint8_t speed) = 0;

    // I2C_SLV* registers (Slave 0-3)
	virtual uint8_t getSlaveAddress(uint8_t num) = 0;
	virtual void setSlaveAddress(uint8_t num, uint8_t address) = 0;
	virtual uint8_t getSlaveRegister(uint8_t num) = 0;
	virtual void setSlaveRegister(uint8_t num, uint8_t reg) = 0;
	virtual bool getSlaveEnabled(uint8_t num) = 0;
	virtual void setSlaveEnabled(uint8_t num, bool enabled) = 0;
	virtual bool getSlaveWordByteSwap(uint8_t num) = 0;
	virtual void setSlaveWordByteSwap(uint8_t num, bool enabled) = 0;
	virtual bool getSlaveWriteMode(uint8_t num) = 0;
	virtual void setSlaveWriteMode(uint8_t num, bool mode) = 0;
	virtual bool getSlaveWordGroupOffset(uint8_t num) = 0;
	virtual void setSlaveWordGroupOffset(uint8_t num, bool enabled) = 0;
	virtual uint8_t getSlaveDataLength(uint8_t num) = 0;
	virtual void setSlaveDataLength(uint8_t num, uint8_t length) = 0;

    // I2C_SLV* registers (Slave 4)
    virtual uint8_t getSlave4Address() = 0;
    virtual void setSlave4Address(uint8_t address) = 0;
    virtual uint8_t getSlave4Register() = 0;
    virtual void setSlave4Register(uint8_t reg) = 0;
    virtual void setSlave4OutputByte(uint8_t data) = 0;
    virtual bool getSlave4Enabled() = 0;
    virtual void setSlave4Enabled(bool enabled) = 0;
    virtual bool getSlave4InterruptEnabled() = 0;
    virtual void setSlave4InterruptEnabled(bool enabled) = 0;
    virtual bool getSlave4WriteMode() = 0;
    virtual void setSlave4WriteMode(bool mode) = 0;
    virtual uint8_t getSlave4MasterDelay() = 0;
    virtual void setSlave4MasterDelay(uint8_t delay) = 0;
    virtual uint8_t getSlate4InputByte() = 0;

    // I2C_MST_STATUS register
    virtual bool getPassthroughStatus() = 0;
    virtual bool getSlave4IsDone() = 0;
    virtual bool getLostArbitration() = 0;
    virtual bool getSlave4Nack() = 0;
    virtual bool getSlave3Nack() = 0;
    virtual bool getSlave2Nack() = 0;
    virtual bool getSlave1Nack() = 0;
    virtual bool getSlave0Nack() = 0;

    // INT_PIN_CFG register
    virtual bool getInterruptMode() = 0;
    virtual void setInterruptMode(bool mode) = 0;
    virtual bool getInterruptDrive() = 0;
    virtual void setInterruptDrive(bool drive) = 0;
    virtual bool getInterruptLatch() = 0;
    virtual void setInterruptLatch(bool latch) = 0;
    virtual bool getInterruptLatchClear() = 0;
    virtual void setInterruptLatchClear(bool clear) = 0;
    virtual bool getFSyncInterruptLevel() = 0;
    virtual void setFSyncInterruptLevel(bool level) = 0;
    virtual bool getFSyncInterruptEnabled() = 0;
    virtual void setFSyncInterruptEnabled(bool enabled) = 0;
    virtual bool getI2CBypassEnabled() = 0;
    virtual void setI2CBypassEnabled(bool enabled) = 0;
    virtual bool getClockOutputEnabled() = 0;
    virtual void setClockOutputEnabled(bool enabled) = 0;

    // INT_ENABLE register
    virtual uint8_t getIntEnabled() = 0;
    virtual void setIntEnabled(uint8_t enabled) = 0;
    virtual bool getIntFreefallEnabled() = 0;
    virtual void setIntFreefallEnabled(bool enabled) = 0;
    virtual bool getIntMotionEnabled() = 0;
    virtual void setIntMotionEnabled(bool enabled) = 0;
    virtual bool getIntZeroMotionEnabled() = 0;
    virtual void setIntZeroMotionEnabled(bool enabled) = 0;
    virtual bool getIntFIFOBufferOverflowEnabled() = 0;
    virtual void setIntFIFOBufferOverflowEnabled(bool enabled) = 0;
    virtual bool getIntI2CMasterEnabled() = 0;
    virtual void setIntI2CMasterEnabled(bool enabled) = 0;
    virtual bool getIntDataReadyEnabled() = 0;
    virtual void setIntDataReadyEnabled(bool enabled) = 0;

    // INT_STATUS register
    virtual uint8_t getIntStatus() = 0;
    virtual bool getIntFreefallStatus() = 0;
    virtual bool getIntMotionStatus() = 0;
    virtual bool getIntZeroMotionStatus() = 0;
    virtual bool getIntFIFOBufferOverflowStatus() = 0;
    virtual bool getIntI2CMasterStatus() = 0;
    virtual bool getIntDataReadyStatus() = 0;

    // ACCEL_*OUT_* registers
    virtual void getMotion9(int16_t* ax, int16_t* ay, int16_t* az, int16_t* gx, int16_t* gy, int16_t* gz, int16_t* mx, int16_t* my, int16_t* mz) = 0;
    virtual void getMotion6(int16_t* ax, int16_t* ay, int16_t* az, int16_t* gx, int16_t* gy, int16_t* gz) = 0;
    virtual void getAcceleration(int16_t* x, int16_t* y, int16_t* z) = 0;
    virtual int16_t getAccelerationX() = 0;
    virtual int16_t getAccelerationY() = 0;
    virtual int16_t getAccelerationZ() = 0;

    // TEMP_OUT_* registers
    virtual int16_t getTemperature() = 0;

    // GYRO_*OUT_* registers
    virtual void getRotation(int16_t* x, int16_t* y, int16_t* z) = 0;
    virtual int16_t getRotationX() = 0;
    virtual int16_t getRotationY() = 0;
    virtual int16_t getRotationZ() = 0;

    // EXT_SENS_DATA_* registers
    virtual uint8_t getExternalSensorByte(int position) = 0;
    virtual uint16_t getExternalSensorWord(int position) = 0;
    virtual uint32_t getExternalSensorDWord(int position) = 0;

    // MOT_DETECT_STATUS register
    virtual uint8_t getMotionStatus() = 0;
    virtual bool getXNegMotionDetected() = 0;
    virtual bool getXPosMotionDetected() = 0;
    virtual bool getYNegMotionDetected() = 0;
    virtual bool getYPosMotionDetected() = 0;
    virtual bool getZNegMotionDetected() = 0;
    virtual bool getZPosMotionDetected() = 0;
    virtual bool getZeroMotionDetected() = 0;

    // I2C_SLV*_DO register
    virtual void setSlaveOutputByte(uint8_t num, uint8_t data) = 0;

    // I2C_MST_DELAY_CTRL register
    virtual bool getExternalShadowDelayEnabled() = 0;
    virtual void setExternalShadowDelayEnabled(bool enabled) = 0;
    virtual bool getSlaveDelayEnabled(uint8_t num) = 0;
    virtual void setSlaveDelayEnabled(uint8_t num, bool enabled) = 0;

    // SIGNAL_PATH_RESET register
    virtual void resetGyroscopePath() = 0;
    virtual void resetAccelerometerPath() = 0;
    virtual void resetTemperaturePath() = 0;

    // MOT_DETECT_CTRL register
    virtual uint8_t getAccelerometerPowerOnDelay() = 0;
    virtual void setAccelerometerPowerOnDelay(uint8_t delay) = 0;
    virtual uint8_t getFreefallDetectionCounterDecrement() = 0;
    virtual void setFreefallDetectionCounterDecrement(uint8_t decrement) = 0;
    virtual uint8_t getMotionDetectionCounterDecrement() = 0;
    virtual void setMotionDetectionCounterDecrement(uint8_t decrement) = 0;

    // USER_CTRL register
    virtual bool getFIFOEnabled() = 0;
    virtual void setFIFOEnabled(bool enabled) = 0;
    virtual bool getI2CMasterModeEnabled() = 0;
    virtual void setI2CMasterModeEnabled(bool enabled) = 0;
    virtual void switchSPIEnabled(bool enabled) = 0;
    virtual void resetFIFO() = 0;
    virtual void resetI2CMaster() = 0;
    virtual void resetSensors() = 0;

    // PWR_MGMT_1 register
    virtual void reset() = 0;
    virtual bool getSleepEnabled() = 0;
    virtual void setSleepEnabled(bool enabled) = 0;
    virtual bool getWakeCycleEnabled() = 0;
    virtual void setWakeCycleEnabled(bool enabled) = 0;
    virtual bool getTempSensorEnabled() = 0;
    virtual void setTempSensorEnabled(bool enabled) = 0;
    virtual uint8_t getClockSource() = 0;
    virtual void setClockSource(uint8_t source) = 0;

    // PWR_MGMT_2 register
    virtual uint8_t getWakeFrequency() = 0;
    virtual void setWakeFrequency(uint8_t frequency) = 0;
    virtual bool getStandbyXAccelEnabled() = 0;
    virtual void setStandbyXAccelEnabled(bool enabled) = 0;
    virtual bool getStandbyYAccelEnabled() = 0;
    virtual void setStandbyYAccelEnabled(bool enabled) = 0;
    virtual bool getStandbyZAccelEnabled() = 0;
    virtual void setStandbyZAccelEnabled(bool enabled) = 0;
    virtual bool getStandbyXGyroEnabled() = 0;
    virtual void setStandbyXGyroEnabled(bool enabled) = 0;
    virtual bool getStandbyYGyroEnabled() = 0;
    virtual void setStandbyYGyroEnabled(bool enabled) = 0;
    virtual bool getStandbyZGyroEnabled() = 0;
    virtual void setStandbyZGyroEnabled(bool enabled) = 0;

    // FIFO_COUNT_* registers
    virtual uint16_t getFIFOCount() = 0;

    // FIFO_R_W register
    virtual uint8_t getFIFOByte() = 0;
    virtual void setFIFOByte(uint8_t data) = 0;
    virtual void getFIFOBytes(uint8_t *data, uint8_t length) = 0;

    // WHO_AM_I register
    virtual uint8_t getDeviceID() = 0;
    virtual void setDeviceID(uint8_t id) = 0;
        
    // ======== UNDOCUMENTED/DMP REGISTERS/METHODS ========
        
    // XG_OFFS_TC register
    virtual uint8_t getOTPBankValid() = 0;
    virtual void setOTPBankValid(bool enabled) = 0;
    virtual int8_t getXGyroOffsetTC() = 0;
    virtual void setXGyroOffsetTC(int8_t offset) = 0;

    // YG_OFFS_TC register
    virtual int8_t getYGyroOffsetTC() = 0;
    virtual void setYGyroOffsetTC(int8_t offset) = 0;

    // ZG_OFFS_TC register
    virtual int8_t getZGyroOffsetTC() = 0;
    virtual void setZGyroOffsetTC(int8_t offset) = 0;

    // X_FINE_GAIN register
    virtual int8_t getXFineGain() = 0;
    virtual void setXFineGain(int8_t gain) = 0;

    // Y_FINE_GAIN register
    virtual int8_t getYFineGain() = 0;
    virtual void setYFineGain(int8_t gain) = 0;

    // Z_FINE_GAIN register
    virtual int8_t getZFineGain() = 0;
    virtual void setZFineGain(int8_t gain) = 0;

    // XA_OFFS_* registers
    virtual int16_t getXAccelOffset() = 0;
    virtual void setXAccelOffset(int16_t offset) = 0;

    // YA_OFFS_* register
    virtual int16_t getYAccelOffset() = 0;
    virtual void setYAccelOffset(int16_t offset) = 0;

    // ZA_OFFS_* register
    virtual int16_t getZAccelOffset() = 0;
    virtual void setZAccelOffset(int16_t offset) = 0;

    // XG_OFFS_USR* registers
    virtual int16_t getXGyroOffset() = 0;
    virtual void setXGyroOffset(int16_t offset) = 0;

    // YG_OFFS_USR* register
    virtual int16_t getYGyroOffset() = 0;
    virtual void setYGyroOffset(int16_t offset) = 0;

    // ZG_OFFS_USR* register
    virtual int16_t getZGyroOffset() = 0;
    virtual void setZGyroOffset(int16_t offset) = 0;
        
    // INT_ENABLE register (DMP functions)
    virtual bool getIntPLLReadyEnabled() = 0;
    virtual void setIntPLLReadyEnabled(bool enabled) = 0;
    virtual bool getIntDMPEnabled() = 0;
    virtual void setIntDMPEnabled(bool enabled) = 0;
        
    // DMP_INT_STATUS
    virtual bool getDMPInt5Status() = 0;
    virtual bool getDMPInt4Status() = 0;
    virtual bool getDMPInt3Status() = 0;
    virtual bool getDMPInt2Status() = 0;
    virtual bool getDMPInt1Status() = 0;
    virtual bool getDMPInt0Status() = 0;

    // INT_STATUS register (DMP functions)
    virtual bool getIntPLLReadyStatus() = 0;
    virtual bool getIntDMPStatus() = 0;
        
    // USER_CTRL register (DMP functions)
    virtual bool getDMPEnabled() = 0;
    virtual void setDMPEnabled(bool enabled) = 0;
    virtual void resetDMP() = 0;
        
    // BANK_SEL register
    virtual void setMemoryBank(uint8_t bank, bool prefetchEnabled=false, bool userBank=false) = 0;
        
    // MEM_START_ADDR register
    virtual void setMemoryStartAddress(uint8_t address) = 0;
        
    // MEM_R_W register
    virtual uint8_t readMemoryByte() = 0;
    virtual void writeMemoryByte(uint8_t data) = 0;
    virtual void readMemoryBlock(uint8_t *data, uint16_t dataSize, uint8_t bank=0, uint8_t address=0) = 0;
    virtual bool writeMemoryBlock(const uint8_t *data, uint16_t dataSize, uint8_t bank=0, uint8_t address=0, bool verify=true, bool useProgMem=false) = 0;
    virtual bool writeProgMemoryBlock(const uint8_t *data, uint16_t dataSize, uint8_t bank=0, uint8_t address=0, bool verify=true) = 0;

    virtual bool writeDMPConfigurationSet(const uint8_t *data, uint16_t dataSize) = 0;

    // DMP_CFG_1 register
    virtual uint8_t getDMPConfig1() = 0;
    virtual void setDMPConfig1(uint8_t config) = 0;

    // DMP_CFG_2 register
    virtual uint8_t getDMPConfig2() = 0;
    virtual void setDMPConfig2(uint8_t config) = 0;

	virtual uint8_t dmpInitialize() = 0;
	virtual bool dmpPacketAvailable() = 0;


	// Get Fixed Point data from FIFO
	virtual uint8_t dmpGetAccel(int32_t *data, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetAccel(int16_t *data, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetAccel(VectorInt16 *v, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetQuaternion(int32_t *data, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetQuaternion(int16_t *data, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetQuaternion(QuaternionFloat *q, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetGyro(int32_t *data, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetGyro(int16_t *data, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetGyro(VectorInt16 *v, const uint8_t* packet = 0) = 0;
	virtual uint8_t dmpGetLinearAccel(VectorInt16 *v, VectorInt16 *vRaw, VectorFloat *gravity) = 0;
	virtual uint8_t dmpGetLinearAccelInWorld(VectorInt16 *v, VectorInt16 *vReal, QuaternionFloat *q) = 0;
	virtual uint8_t dmpGetGravity(VectorFloat *v, QuaternionFloat *q) = 0;
	virtual uint8_t dmpGetEuler(float *data, QuaternionFloat *q) = 0;
	virtual uint8_t dmpGetYawPitchRoll(float *data, QuaternionFloat *q, VectorFloat *gravity) = 0;

	virtual uint8_t dmpProcessFIFOPacket(const unsigned char *dmpData) = 0;
	virtual uint8_t dmpReadAndProcessFIFOPacket(uint8_t numPackets, uint8_t *processed = NULL) = 0;

	virtual uint16_t dmpGetFIFOPacketSize() = 0;
};
