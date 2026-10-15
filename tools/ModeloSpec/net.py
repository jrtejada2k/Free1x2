# Modelos por lectura de dotnet/runtime release/8.0 (no se ejecuta .NET):
#  - ArraySortHelper.cs (IntrospectiveSort con Comparison<T>) + SortUtils.MoveNansToFront
#  - Hashtable.cs + HashHelpers.cs
import math, struct
def f32(x): return struct.unpack('f', struct.pack('f', x))[0]

def introsort(keys, cmp):
    n=len(keys)
    if n>1: _intro(keys,0,n,2*(n.bit_length()-1+1),cmp)
def _swapifgreater(k,cmp,i,j):
    if cmp(k[i],k[j])>0: k[i],k[j]=k[j],k[i]
def _intro(k,lo,size,depth,cmp):
    while size>1:
        if size<=16:
            if size==2: _swapifgreater(k,cmp,lo,lo+1); return
            if size==3:
                _swapifgreater(k,cmp,lo,lo+1); _swapifgreater(k,cmp,lo,lo+2); _swapifgreater(k,cmp,lo+1,lo+2); return
            _insertion(k,lo,size,cmp); return
        if depth==0: _heap(k,lo,size,cmp); return
        depth-=1
        p=_partition(k,lo,size,cmp)
        _intro(k,lo+p+1,size-(p+1),depth,cmp)
        size=p
def _partition(k,lo,size,cmp):
    hi=size-1; mid=hi>>1
    _swapifgreater(k,cmp,lo,lo+mid); _swapifgreater(k,cmp,lo,lo+hi); _swapifgreater(k,cmp,lo+mid,lo+hi)
    pivot=k[lo+mid]; k[lo+mid],k[lo+hi-1]=k[lo+hi-1],k[lo+mid]
    left,right=0,hi-1
    while left<right:
        left+=1
        while cmp(k[lo+left],pivot)<0: left+=1
        right-=1
        while cmp(pivot,k[lo+right])<0: right-=1
        if left>=right: break
        k[lo+left],k[lo+right]=k[lo+right],k[lo+left]
    if left!=hi-1: k[lo+left],k[lo+hi-1]=k[lo+hi-1],k[lo+left]
    return left
def _heap(k,lo,n,cmp):
    for i in range(n>>1,0,-1): _down(k,lo,i,n,cmp)
    for i in range(n,1,-1):
        k[lo],k[lo+i-1]=k[lo+i-1],k[lo]; _down(k,lo,1,i-1,cmp)
def _down(k,lo,i,n,cmp):
    d=k[lo+i-1]
    while i<=n>>1:
        c=2*i
        if c<n and cmp(k[lo+c-1],k[lo+c])<0: c+=1
        if not cmp(d,k[lo+c-1])<0: break
        k[lo+i-1]=k[lo+c-1]; i=c
    k[lo+i-1]=d
def _insertion(k,lo,n,cmp):
    for i in range(n-1):
        t=k[lo+i+1]; j=i
        while j>=0 and cmp(t,k[lo+j])<0:
            k[lo+j+1]=k[lo+j]; j-=1
        k[lo+j+1]=t
def sort_doubles(a):
    left=0
    for i in range(len(a)):
        if a[i]!=a[i]: a[left],a[i]=a[i],a[left]; left+=1
    rest=a[left:]
    introsort(rest,lambda x,y:(x>y)-(x<y))
    a[left:]=rest

PRIMES=[3,7,11,17,23,29,37,47,59,71,89,107,131,163,197,239,293,353,431,521,631,761,919,1103,1327,1597,1931,2333,2801,3371,4049,4861,5839,7013,8419,10103,12143,14591,17519,21023,25229,30293,36353,43627,52361,62851,75431,90523,108631,130363,156437,187751,225307,270371,324449,389357,467237,560689,672827,807403,968897,1162687,1395263,1674319,2009191,2411033,2893249,3471899,4166287,4999559,5999471,7199369]
def isprime(c):
    if c&1:
        for d in range(3,int(math.isqrt(c))+1,2):
            if c%d==0: return False
        return True
    return c==2
def getprime(m):
    for p in PRIMES:
        if p>=m: return p
    i=m|1
    while i<2**31-1:
        if isprime(i) and (i-1)%101!=0: return i
        i+=2
    return m
def expandprime(old):
    new=2*old
    if new>0x7FFFFFC3 and 0x7FFFFFC3>old: return 0x7FFFFFC3
    return getprime(new)
LF=f32(0.72)
def loadsize(n): return int(f32(LF*f32(float(n))))
def h(key):
    t,v=key
    if t=='i32': return v & 0xFFFFFFFF
    lo=v & 0xFFFFFFFF; hi=(v>>32)&0xFFFFFFFF
    return lo^hi
class Hashtable:
    def __init__(s):
        s.b=[[None,None,0] for _ in range(3)]  # key,val,hash_coll(uint32 con bit 31 = colisión)
        s.count=0; s.occ=0; s.load=loadsize(3)
    def _init(s,key,size):
        hc=h(key)&0x7FFFFFFF; seed=hc; incr=1+((seed*101)&0xFFFFFFFF)%(size-1); return hc,seed,incr
    def insert(s,key,val,add):
        if s.count>=s.load: s._rehash(expandprime(len(s.b)))
        elif s.occ>s.load and s.count>100: s._rehash(len(s.b))
        n=len(s.b); hc,seed,incr=s._init(key,n); bn=seed%n; ntry=0
        while True:
            bk=s.b[bn]
            if bk[0] is None:
                bk[1]=val; bk[0]=key; bk[2]|=hc; s.count+=1; return
            if (bk[2]&0x7FFFFFFF)==hc and bk[0]==key:
                if add: raise Exception('ArgumentException')
                bk[1]=val; return
            if not bk[2]&0x80000000:
                bk[2]|=0x80000000; s.occ+=1
            bn=(bn+incr)%n; ntry+=1
            if ntry>=n: raise Exception('InvalidOperation')
    def _rehash(s,newsize):
        s.occ=0; nb=[[None,None,0] for _ in range(newsize)]
        for bk in s.b:
            if bk[0] is not None: s._put(nb,bk[0],bk[1],bk[2]&0x7FFFFFFF)
        s.b=nb; s.load=loadsize(newsize)
    def _put(s,nb,key,val,hc):
        n=len(nb); seed=hc; incr=1+((seed*101)&0xFFFFFFFF)%(n-1); bn=seed%n
        while True:
            if nb[bn][0] is None:
                nb[bn][1]=val; nb[bn][0]=key; nb[bn][2]|=hc; return
            if not nb[bn][2]&0x80000000:
                nb[bn][2]|=0x80000000; s.occ+=1
            bn=(bn+incr)%n
    def get(s,key):
        n=len(s.b); hc,seed,incr=s._init(key,n); bn=seed%n; ntry=0
        while True:
            bk=s.b[bn]
            if bk[0] is None: return None
            if (bk[2]&0x7FFFFFFF)==hc and bk[0]==key: return bk[1]
            bn=(bn+incr)%n; ntry+=1
            if not (bk[2]&0x80000000 and ntry<n): return None
    def contains(s,key): return s.get(key) is not None
    def keys(s): return [bk[0] for bk in reversed(s.b) if bk[0] is not None]


# --- System.Random(int seed): Random.Net5CompatSeedImpl (dotnet/runtime release/8.0), por lectura ---
class RandomNet:
    MBIG = 2**31 - 1
    MSEED = 161803398

    def __init__(s, seed):
        def i32(x):
            x &= 0xFFFFFFFF
            return x - 2**32 if x >= 2**31 else x
        s.t = [0] * 56
        sub = 2**31 - 1 if seed == -2**31 else abs(seed)
        mj = i32(s.MSEED - sub)
        s.t[55] = mj
        mk = 1
        ii = 0
        for i in range(1, 55):
            ii += 21
            if ii >= 55: ii -= 55
            s.t[ii] = mk
            mk = i32(mj - mk)
            if mk < 0: mk += s.MBIG
            mj = s.t[ii]
        for k in range(1, 5):
            for i in range(1, 56):
                n = i + 30
                if n >= 55: n -= 55
                s.t[i] = i32(s.t[i] - s.t[1 + n])
                if s.t[i] < 0: s.t[i] += s.MBIG
        s.inext, s.inextp = 0, 21

    def internal(s):
        s.inext = 1 if s.inext + 1 >= 56 else s.inext + 1
        s.inextp = 1 if s.inextp + 1 >= 56 else s.inextp + 1
        r = s.t[s.inext] - s.t[s.inextp]
        if r == s.MBIG: r -= 1
        if r < 0: r += s.MBIG
        s.t[s.inext] = r
        return r

    def sample(s): return s.internal() * (1.0 / s.MBIG)
    def next(s): return s.internal()
    def next_max(s, m): return int(s.sample() * m)
    def next_rango(s, a, b):
        rango = b - a
        if rango <= 2**31 - 1:
            return int(s.sample() * rango) + a
        r = s.internal()
        if s.internal() % 2 == 0: r = -r
        d = (r + (2**31 - 2)) / (2 * (2**31 - 1) - 1)
        return int(int(d * rango) + a)
    def next_double(s): return s.sample()
