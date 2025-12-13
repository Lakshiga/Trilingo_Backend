# Activity Management Guide

## Overview
Activities are the core learning content in the Trilingo platform. Each activity belongs to a Stage, Main Activity, and Activity Type.

## Creating Activities

### Steps to Create an Activity:
1. Navigate to the Activity Editor page
2. Select the Stage (Level > Stage)
3. Choose the Main Activity (Listening, Speaking, Reading, Writing)
4. Select the Activity Type (Letter Tracking, Word Matching, etc.)
5. Enter multilingual titles (English, Tamil, Sinhala)
6. Configure the activity JSON data
7. Save the activity

### Activity JSON Structure
Each activity type has a specific JSON structure defined in the Activity Type's `jsonMethod` field. This JSON template defines:
- The activity's data structure
- Required fields
- Default values
- Exercise configurations

## Editing Activities
- Click the edit button on any activity
- Modify the title, JSON data, or other properties
- Save changes to update the activity

## Activity Types
Activity Types define the method/template for activities:
- Letter Tracking: For tracing letters
- Word Matching: For matching words
- Sentence Building: For constructing sentences
- And more...

Each Activity Type has a JSON method that serves as a template for activities of that type.

## Best Practices
- Always validate JSON before saving
- Use the preview feature to test activities
- Keep JSON structures consistent within the same Activity Type
- Test activities on mobile devices before publishing

