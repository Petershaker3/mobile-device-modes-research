package com.petershaker.devicetoolkit.installtest;

import android.app.Activity;
import android.os.Bundle;
import android.view.Gravity;
import android.widget.TextView;

public final class TestActivity extends Activity {
    @Override
    protected void onCreate(Bundle state) {
        super.onCreate(state);
        TextView message = new TextView(this);
        message.setGravity(Gravity.CENTER);
        message.setTextSize(22);
        message.setText("Device Toolkit install test\n\ncom.petershaker.devicetoolkit.installtest");
        setContentView(message);
    }
}
