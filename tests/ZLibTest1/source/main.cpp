/*
Copyright (c) 2010-2019, Mark Final
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of BuildAMation nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#include "zlib.h"

#include <iostream>
#include <cctype>

#define CHUNK_SIZE 16384

int main()
{
    std::cout << "Zlib version: " << zlibVersion() << std::endl;

    char in_buffer[] = "Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello";
    char out_buffer[CHUNK_SIZE];

    z_stream stream;
    stream.zalloc = Z_NULL;
    stream.zfree = Z_NULL;
    stream.opaque = Z_NULL;
    int compressionLevel = Z_DEFAULT_COMPRESSION;
    if (Z_OK != deflateInit(&stream, compressionLevel))
    {
        return -1;
    }

    const int originalLength = strlen(in_buffer);
    stream.avail_in = originalLength;
    stream.next_in = (Bytef*)in_buffer;

    std::cout << "Original size: " << originalLength << std::endl;

    stream.avail_out = CHUNK_SIZE;
    stream.next_out = (Bytef*)out_buffer;

    // there are multiple 'success codes' here
    if (Z_STREAM_ERROR == deflate(&stream, Z_FINISH))
    {
        return -1;
    }

    int compressedLength = CHUNK_SIZE - stream.avail_out;
    std::cout << "Compressed size: " << compressedLength << " - " << 100 * static_cast<float>(compressedLength)/originalLength <<  "% of original size" << std::endl;

    for (int i = 0; i < compressedLength; ++i)
    {
        char c = out_buffer[i];
        std::cout << i << " : " << static_cast<int>(c);
        if (std::isalnum(c))
        {
            std::cout << " (" << c << ")";
        }
        std::cout << std::endl;
    }

    if (stream.avail_in > 0)
    {
        // there is more data
        return -1;
    }

    if (Z_OK != deflateEnd(&stream))
    {
        return -1;
    }

    return 0;
}
