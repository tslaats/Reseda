#! /bin/sh

# Close STDOUT file descriptor
exec 1<&-

rm -f examples.js

# Open STDOUT for read and write.
exec 1<>examples.js

echo 'export const examples = '

echo "{ 'Choose an example process': '' "

find examples ../../../wintertypes4dcr/sydney/examples -type f -name \\[\* -print0 | while read -d $'\0' i
do
    /bin/echo -n ","
    TITLE=$(basename "$i" | cut -d. -f1)
    /bin/echo -n "'$TITLE': \`"
    cat "$i" #| sed 's/@/\\@/g'
    /bin/echo "\`"
done

echo "}"

