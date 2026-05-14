OlympicInterviewerConsole solution contains data-driven coded UI test which performs actions of interviewers who pass CATI Console interview. 

Preparation of data

Open ..\OlympicData\OlympicData.xml. Each <interview> tag contains data for 1 interview.
This data is described below.

<location> - put location of CatiInterviewerConsole.exe which is planned to be under test

<name> - name of interviewer

<password> - password of interviewer

<labelSingle> - shouldn't be changed cause it's text of single question

<RadioButton1Selected>, <RadioButton2Selected>, <otherRadioButtonSelected> - put "false" or "true" to choose which radio button should be selected in single question. 

If <otherRadioButtonSelected> has value "true", then
specify text of other answer in <singleOtherText>

<labelMulti> - shouldn't be changed cause it's text of multi question

<CheckBox1Checked>, <CheckBox2Checked>, <otherCheckBoxChecked> - put "false" or "true" to choose
which check box should be selected in multi question. 

If <otherCheckBoxChecked> has value "true", then
specify text of other answer in <multiOtherText>

<labelNum>, <labelOpen> - shouldn't be changed cause they contain text of numeric and open questions
respectively

<numText>, <openText> can be changed to introduce answers for numeric and open questions respectively




