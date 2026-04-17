CLAUDE.md for \_GenTools



Protocol for new GitHub repo:

1. var PATH\_TO\_SOURCE (e.g., C:\\Users\\BT\\source\\repos\\BiffSully\\)
2. var LOCAL\_PROJECT (e.g., \_GenTools)
3. var NEW\_REPO (same as LOCAL\_PROJECT unless user says otherwise)
4. use GitHub account DaveKaplan (prompt for pwd if necessary)
5. ensure NEW\_REPO isn't already on GitHub (prompt for name if it is)
6. rename LOCAL\_PROJECT folder to "\_\_TEMP\_" + LOCAL\_PROJECT
7. create NEW\_REPO under DaveKaplan/ (default) or BiffSully/ (if user specifies)
8. clone NEW\_REPO locally in PATH\_TO\_SOURCE
9. build NEW\_REPO
10. prompt user to test the build
11. ask user whether to delete "\_\_TEMP\_" + LOCAL\_PROJECT folder \[Yes | No]

