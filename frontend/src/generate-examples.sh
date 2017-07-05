#! /bin/sh

# Close STDOUT file descriptor
exec 1<&-

# Open STDOUT for read and write.
exec 1<>examples.js

echo 'export const examples = '

echo "{ 'Choose an example process': '' "

find examples ../../../wintertypes4dcr/losangeles/examples -type  f -print0 | while read -d $'\0' i
do
    /bin/echo -n ","
    TITLE=$(basename "$i")
    /bin/echo -n "'$TITLE': \`"
    cat "$i" | sed "s/@/\\@/g"
    /bin/echo "\`"
done

echo "}"

